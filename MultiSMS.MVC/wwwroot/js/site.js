function CalculateHeaderWidthAndToolbarHeight() {

    const sidebar = document.getElementById('toolbar');
    const header = document.getElementById('main-header');
    const renderedContent = document.getElementById('content-render');

    header.style.width = `${renderedContent.offsetWidth + 50}px`;

    renderedContent.style.height = 'auto';
    sidebar.style.height = `auto`;

    if (sidebar.offsetHeight > 0) {
        if (renderedContent.offsetHeight > sidebar.offsetHeight) {
            sidebar.style.height = `${renderedContent.offsetHeight}px`;
        }
        else {
            renderedContent.style.height = `${sidebar.offsetHeight}px`
        }
    }

}

function OnInputFilterTable(searchBarIdentification, tableIdentification) {
    $(searchBarIdentification).on('input', function () {

        var filterText = $(this).val().toLowerCase();

        $(`${tableIdentification} tbody tr`).each(function () {
            var rowHit = false;
            $(this).find('td').each(function () {
                if ($(this).text().toLowerCase().includes(filterText)) {
                    rowHit = true;
                    return false;
                }
            });

            $(this).toggle(rowHit);
        });
    });
}

function SendSMS(text, chosenGroupId, chosenGroupName, additionalPhoneNumbers, additionalInfo) {
    $.ajax({
        url: '/SmsApi/SendSmsMessage',
        type: 'GET',
        contentType: 'application/json',
        data: { text: text, chosenGroupId: chosenGroupId, chosenGroupName: chosenGroupName, additionalPhoneNumbers: additionalPhoneNumbers, additionalInfo: additionalInfo },
        success: function (response) {
            if (response.status == "failed") {
                console.log(`Operation failed, server responded with code ${response.code} - ${response.message}`);
                $("#status-message-sms").addClass("failed-status");
                $("#status-message-sms").html(`Operacja zakończyła się niepowodzeniem, błąd ${response.code} - ${response.message} <button type='button' class='btn-close text-dark' aria-label='Close' onclick='CloseAlert()'></button>`);
                $("#status-message-sms").show();
            }
            else {
                $("#status-message-sms").addClass("success-status");
                console.log(`Operation successful, messages queued: ${response.queued}, messages unsent: ${response.unsent}`);
                $("#status-message-sms").html(`Wiadomość została wysłana (zakolejkowane: ${response.queued}, niewysłane: ${response.unsent}) <button type='button' class='btn-close text-dark' onclick='CloseAlert()' aria-label='Close'></button>`);
                $("#status-message-sms").show();
                $("#send-sms-form :input").val('');
                document.getElementById('sms-text-symbol-counter').innerHTML = "";
                $("#GroupOfContacts").removeAttr("data-id");
                $(".input-icons").hide();

            }
        },
        error: function (error) {
            $("#status-message-sms").addClass("failed-status");
            $("#status-message-sms").html(`Wystąpił wewnętrzny błąd servera! Skontaktuj się z obsługą. <button type='button' class='btn-close text-dark' aria-label='Close' onclick='CloseAlert()'></button>`);
            $("#status-message-sms").show();
            console.error(error.message);
        }
    });
}

function checkIfAuthorizationSuccessful(password) {
    $.ajax({
        url: '/Home/CheckIfAuthorizationSuccessful',
        type: 'GET',
        data: { password: password },
        contentType: 'application/json',
        success: function (response) {
            if (response.includes("Nieudana")) {
                $("#authorization-failed-message").html(response);
                $("#settings-password-input").val("");
            }
            else {
                $("#settings-form").append(response);
                $("#authorization-failed-message").html("");
                $("#settings-password-input").val("");
                $("#settings-password").hide();
                $("#settings-container").show();

            }
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function updateApiSettings() {
    var activeApi = $("#select-active-api").val();
    var senderName = $("#api-sender-name").val();
    var fastChannel = $("#fast-channel-checkbox").prop("checked") ? true : false;
    var testMode = $("#test-mode-checkbox").prop("checked") ? true : false;

    $.ajax({
        url: '/Home/UpdateApiSettings',
        type: 'POST',
        data: JSON.stringify({ ActiveApiName: activeApi, SenderName: senderName, FastChannel: fastChannel, TestMode: testMode }),
        contentType: 'application/json; charset=utf-8',
        success: function () {
            console.log("Api settings updated successfuly");
        },
        error: function (errorData) {
            console.error(errorData);
        }
    });
}

function importContacts() {

    function IterateOverListOfObjectsAndAppendToTable(object, tableId) {
        $.each(object.ListOfEmployees, function (index, employee) {
            var group = employee.employeeGroupNames;
            var email = employee.email;

            group = (group == null || group.length === 0) ? "Nie przypisano" : group.join(", ");
            email = (email == null || email === "") ? "Brak danych" : email;

            var statusClass = employee.isActive ? 'active-pill' : 'inactive-pill';
            var statusText = employee.isActive ? 'Aktywny' : 'Nieaktywny';

            var newRow = `<tr>
                        <td class="tiny-cell">${employee.name}</td>
                        <td class="tiny-cell">${employee.surname}</td>
                        <td class="small-cell">${email}</td>
                        <td class="small-cell">${employee.phoneNumber}</td>
                        <td class="tiny-centered-cell"><span class="${statusClass}">${statusText}</span></td>`;

            if (typeof object.ListOfNonExistantGroupIds == 'undefined' && tableId == "added-contacts-table") {
                newRow += `<td class="centered-cell">${group}</td>
                            <td class="tiny-centered-cell">Brak</td>
                            </tr>`;
            }
            else if (typeof object.ListOfNonExistantGroupIds != 'undefined') {
                var nonExistantGroups = object.ListOfNonExistantGroupIds;
                var nonExistentGroupsForContact = nonExistantGroups[index];

                newRow += `<td class="centered-cell">${group}</td>`;

                if (nonExistentGroupsForContact.length == 0 || typeof nonExistentGroupsForContact == "string" || nonExistentGroupsForContact == "") {
                    newRow += `<td class="tiny-centered-cell">Brak</td>
                                </tr>`;
                }
                else {
                    newRow += `<td class="tiny-centered-cell">${nonExistentGroupsForContact.join(', ')}</td>
                                </tr>`;
                }
            }
            $(`#${tableId} tbody`).append(newRow);
        });
    }

    function importFile() {
        var fileInput = document.getElementById('import-file-input');
        var file = fileInput.files[0];

        if (!file) {
            resetFileInput();
            return;
        }

        var fileName = file.name;

        if (!isValidFileExtension(fileName, '.csv')) {
            handleInvalidFileType();
            return;
        }

        var formData = createFormData(file);

        $.ajax({
            url: '/Home/ImportContacts',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: handleImportSuccess,
            error: handleImportError
        });
    }

    function isValidFileExtension(fileName, validExtension) {
        return fileName.toLowerCase().endsWith(validExtension);
    }

    function resetFileInput() {
        var fileInput = document.getElementById('import-file-input');
        fileInput.value = null;
    }

    function createFormData(file) {
        var formData = new FormData();
        formData.append('file', file);
        return formData;
    }

    function handleInvalidFileType() {
        console.log("Unsupported file type! Import aborted.");
        $("#import-modal-content-text").text("Nieprawidłowy format pliku! Import przerwany.");
        $("#closeModal").show();
        $("#import-modal-footer").addClass("justify-content-center");

        $("#closeModal").on("click", function () {
            $("#viewReportModal").modal("hide");
        });

        resetFileInput();
    }

    function handleImportSuccess(responseObject) {
        var addedLength;

        clearTableRows(['repeated-contacts-table', 'invalid-contacts-table', 'added-contacts-table']);

        if (responseObject.status === "Failure") {
            handleImportFailure(responseObject.message);
            return;
        }

        $("#import-modal-content-text").text("Import zakończony powodzeniem. Tworzenie raportu...");

        handleRepeatedContacts(responseObject.repeatedEmployees);
        handleInvalidContacts(responseObject.invalidEmployees);
        addedLength = handleAddedContacts(responseObject.status, responseObject.addedEmployees, responseObject.nonExistantGroupIds);

        $("#import-modal-content-text").text(`${responseObject.message} (dodano: ${addedLength}, duplikaty: ${responseObject.repeatedEmployees.length}, nieprawidłowe: ${responseObject.invalidEmployees.length})`);
        $("#closeModal").show();
        $("#viewReport").show();

        attachEventHandlers();
        resetFileInput();
    }

    function clearTableRows(tableIds) {
        tableIds.forEach(function (tableId) {
            $(`#${tableId} tbody`).empty();
        });
    }

    function handleImportFailure(errorMessage) {
        $("#import-modal-content-text").text(errorMessage);
        $("#closeModal").show();
        $("#import-modal-footer").addClass("justify-content-center");
        resetFileInput();

        attachEventHandlers();
    }

    function handleRepeatedContacts(repeatedEmployees) {
        if (repeatedEmployees.length === 0) {
            $("#import-repeated-contacts-container").hide();
        } else {
            IterateOverListOfObjectsAndAppendToTable({ ListOfEmployees: repeatedEmployees }, "repeated-contacts-table");
        }
    }

    function handleInvalidContacts(invalidEmployees) {
        if (invalidEmployees.length === 0) {
            $("#import-invalid-contacts-container").hide();
        } else {
            IterateOverListOfObjectsAndAppendToTable({ ListOfEmployees: invalidEmployees }, "invalid-contacts-table");
        }
    }

    function handleAddedContacts(status, addedEmployees, nonExistantGroupIds) {
        var addedLength = addedEmployees ? addedEmployees.length : 0;

        if (addedLength == 0) {
            $("#import-added-contacts-container").hide();
        }

        if (status === "Success") {
            IterateOverListOfObjectsAndAppendToTable({ ListOfEmployees: addedEmployees }, "added-contacts-table");
        } else if (status === "Partial Success") {
            IterateOverListOfObjectsAndAppendToTable({ ListOfEmployees: addedEmployees, ListOfNonExistantGroupIds: nonExistantGroupIds }, "added-contacts-table");
        }

        return addedLength;
    }

    function attachEventHandlers() {
        $("#closeModal").on("click", function () {
            FinishBrowsingImportReport();
            $("#viewReportModal").modal("hide");
        });

        $("#viewReport").on("click", function () {
            $("#viewReportModal").modal("hide");
            $(".contacts-list-container").hide();
            $(".import-report-container").show();
        });
    }

    function handleImportError(error) {
        console.error(error);
        $("#closeModal").show();
        $("#import-modal-content-text").text("Wystąpił niespodziewany błąd!");
        $("#import-modal-footer").addClass("justify-content-center");
        $("#viewReport").hide();

        $("#closeModal").on("click", function () {
            $("#viewReportModal").modal("hide");
            $("#import-modal-footer").removeClass("justify-content-center");

        });

        resetFileInput();
    }

    importFile();
}

function ExportContactsAndDownloadExcel() {
    $.ajax({
        url: '/Home/DownloadExcelWithContacts',
        type: 'GET',
        contentType: 'application/json',
        xhrFields: {
            responseType: 'blob'
        },
        success: function (responseFileData) {
            var blob = new Blob([responseFileData], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            var downloadLink = document.createElement('a');
            downloadLink.href = window.URL.createObjectURL(blob);
            downloadLink.download = "Multi-SMS Kontakty.xlsx"

            downloadLink.dispatchEvent(new MouseEvent('click'));
            window.URL.revokeObjectURL(downloadLink.href);
        },
        error: function (error) {
            console.error("Export has failed");
        }
    });
}

function HandleTemplateTypeLog(templateEntity) {

    var relatedObjects =
        `<span class="log-related-object-title">Szablon</span>
                <hr style="margin-top: 20px";/>
                <span class="form-subtitle">Informacje</span>
                <div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logTemplate-TemplateIdDetail">Id:</label>
                        <span class="details-span" id="logTemplate-TemplateIdDetail">${templateEntity.templateId}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logTemplate-TemplateNameDetail">Nazwa:</label>
                        <span class="details-span" id="logTemplate-TemplateNameDetail">${templateEntity.templateName}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logTemplate-TemplateDescriptionDetail">Opis:</label>
                        <span class="details-span" id="logTemplate-TemplateDescriptionDetail">${templateEntity.templateDescription == null ? "Brak opisu" : templateEntity.templateDescription}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logTemplate-TemplateContentDetail">Treść:</label>
                        <span class="details-span" id="logTemplate-TemplateContentDetail">${templateEntity.templateContent}</span>
                    </div>
                </div>
                `;

    $("#log-related-objects-data-form-group").append(relatedObjects);
}

function HandleGroupsAssignTypeLog(groupAssignObject) {

    var groupDescription = groupAssignObject.group.groupDescription;

    var email = groupAssignObject.contact.email;
    var hqAddress = groupAssignObject.contact.hqAddress;
    var postalNumber = groupAssignObject.contact.postalNumber;
    var city = groupAssignObject.contact.city;
    var department = groupAssignObject.contact.department;

    var relatedObjects =
        `<span class="log-related-object-title">Grupa</span>
                <hr style="margin-top: 20px";/>
                <span class="form-subtitle">Informacje</span>
                <div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-GroupIdDetail">Id:</label>
                        <span class="details-span" id="logGroupAssign-GroupIdDetail">${groupAssignObject.group.groupId}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-GroupNameDetail">Nazwa:</label>
                        <span class="details-span" id="logGroupAssign-GroupNameDetail">${groupAssignObject.group.groupName}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-GroupDescriptionDetail">Opis:</label>
                        <span class="details-span" id="logGroupAssign-GroupDescriptionDetail">${(groupDescription == null || groupDescription == "") ? "Brak opisu" : groupDescription}</span>
                    </div>
                </div>
                <hr />
                <span class="log-related-object-title">Kontakt</span>
                <hr style="margin-top: 20px";/>
                <span class="form-subtitle">Dane personalne</span>
                <div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeeIdDetail">Id:</label>
                        <span class="details-span" id="logGroupAssign-EmployeeIdDetail">${groupAssignObject.contact.employeeId}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeeNameDetail">Imię:</label>
                        <span class="details-span" id="logGroupAssign-EmployeeNameDetail">${groupAssignObject.contact.name}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeeSurnameDetail">Nazwisko:</label>
                            <span class="details-span" id="logGroupAssign-EmployeeSurnameDetail">${groupAssignObject.contact.surname}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeeEmailDetail">Email:</label>
                            <span class="details-span" id="logGroupAssign-EmployeeEmailDetail">${(email == null || email == "") ? "Brak danych" : email}</span>
                    </div>
                </div>
                <hr />
                <span class="form-subtitle">Dane kontaktowe</span>
                    <div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeePhonenumberDetail">Numer telefonu:</label>
                            <span class="details-span" id="logGroupAssign-EmployeePhonenumberDetail">${groupAssignObject.contact.phoneNumber}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeeHQAddressDetail">Adres miejsca pracy:</label>
                        <span class="details-span" id="logGroupAssign-EmployeeHQAddressDetail">${(hqAddress == null || hqAddress == "") ? "Brak danych" : hqAddress}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeePostalNumberDetail">Kod pocztowy:</label>
                        <span class="details-span" id="logGroupAssign-EmployeePostalNumberDetail">${(postalNumber == null || postalNumber == "") ? "Brak danych" : postalNumber}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeeCityDetail">Miasto:</label>
                        <span class="details-span" id="logGroupAssign-EmployeeCityDetail">${(city == null || city == "") ? "Brak danych" : city}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeeDepartmentDetail">Jednostka organizacyjna:</label>
                        <span class="details-span" id="logGroupAssign-EmployeeDepartmentDetail">${(department == null || department == "") ? "Brak danych" : department}</span>
                    </div>
                </div>
                <hr />
                    <div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeeStatusDetail">Status:</label>
                            <span class="details-span" id="logGroupAssign-EmployeeStatusDetail">${groupAssignObject.contact.isActive ? "Aktywny" : "Nieaktywny"}</span>
                    </div>
                </div>
                `;

    $("#log-related-objects-data-form-group").append(relatedObjects);

    $("#logGroupAssign-EmployeeStatusDetail").text() == "Aktywny" ? $("#logGroupAssign-EmployeeStatusDetail").css('color', '#018707') : $("#logGroupAssign-EmployeeStatusDetail").css('color', '#df3b15');

    $(".logs-detail-label").css({
        "min-width": "172px",
        "max-width": "172px"
    });
}

function handleImportTypeLog(importEntity) {
    console.log(importEntity);
    var importStatus;

    if (importEntity.importStatus == "Success") {
        importStatus = "Sukces";
    }
    else if (importEntity.importStatus == "Partial Success") {
        importStatus = "Częściowy sukces";
    }
    else {
        importStatus = "Niepowodzenie";
    }

    var relatedObjects =
        `<span class="log-related-object-title">Import</span>
            <hr style="margin-top: 20px";/>
            <span class="form-subtitle">Informacje</span>
            <div class="row mb-10" style="margin-top: 20px;">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logImport-StatusDetail">Status:</label>
                    <span class="details-span" id="logImport-StatusDetail">${importStatus}</span>
                </div>
            </div>
            <div class="row mb-10" style="margin-top: 20px;">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logImport-messageDetail">Wiadomość:</label>
                    <span class="details-span" id="logImport-messageDetail">${importEntity.importMessage}</span>
                </div>
            </div>
            <div id="log-import-tables">
                <hr style="margin-top:20px;"/>
                <span class="form-subtitle">Tabelaryczny rozkład kontaktów</span>
                <br />
                <div style="margin-top: 20px;">
                    <span class="logs-detail-label" id="log-added-contacts-title"">Dodane kontakty:</span>
                </div>
                <div class="table-responsive" style="border: 2px solid #f0f1f1; margin-top:20px; margin-bottom: 20px; max-height: 250px;">
                    <table class="log-import-members-list table table-borderless" id="log-detail-import-addedContacts-table">
                        <thead>
                            <tr>
                                <th class="tiny-cell">Imię</th>
                                <th class="tiny-cell">Nazwisko</th>
                                <th class="small-cell" style="width: 150px;">Numer telefonu</th>
                                <th class="tiny-centered-cell">Status</th>
                                <th class="tiny-centered-cell" style="min-width: 185px;">Nieprawidłowe grupy</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <span class="logs-detail-label" id="log-repeated-contacts-title">Powtórzone kontakty:</span>
                <div class="table-responsive" style="border: 2px solid #f0f1f1; margin-top:20px; margin-bottom: 20px; max-height: 250px;">
                    <table class="log-import-members-list table table-borderless" id="log-detail-import-repeatedContacts-table">
                        <thead>
                            <tr>
                                <th class="tiny-cell">Imię</th>
                                <th class="tiny-cell">Nazwisko</th>
                                <th class="small-cell" style="width: 150px;">Numer telefonu</th>
                                <th class="tiny-centered-cell">Status</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <span class="logs-detail-label" id="log-invalid-contacts-title">Nieprawidłowe kontakty:</span>
                <div class="table-responsive" style="border: 2px solid #f0f1f1; margin-top:20px; margin-bottom: 20px; max-height: 250px;">
                    <table class="log-import-members-list table table-borderless" id="log-detail-import-invalidContacts-table">
                        <thead>
                            <tr>
                                <th class="tiny-cell">Imię</th>
                                <th class="tiny-cell">Nazwisko</th>
                                <th class="small-cell" style="width: 150px;">Numer telefonu</th>
                                <th class="tiny-centered-cell">Status</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
            `;

    $("#log-related-objects-data-form-group").append(relatedObjects);

    if (importEntity.importStatus == "Failure") {
        $("#log-import-tables").hide();
    }

    if (importEntity.addedEmployees != null && importEntity.addedEmployees.length > 0) {

        $("#log-details-window").css('max-width', '750px');

        $.each(importEntity.addedEmployees, function (index, employee) {

            var statusClass = employee.isActive ? 'active-pill' : 'inactive-pill';
            var statusText = employee.isActive ? 'Aktywny' : 'Nieaktywny';

            var newRow = `<tr>
                                <td class="tiny-cell">${employee.name}</td>
                                <td class="tiny-cell">${employee.surname}</td>
                                <td class="small-cell">${employee.phoneNumber}</td>
                                <td class="tiny-centered-cell"><span class="${statusClass}">${statusText}</span></td>
                                <td class="tiny-centered-cell">${importEntity.nonExistantGroupIds[index] == "" ? "Brak" : importEntity.nonExistantGroupIds[index].join(', ')}</td>
                                </tr>`

            $("#log-detail-import-addedContacts-table tbody").append(newRow);
        });
    }
    else {
        $("#log-detail-import-addedContacts-table").parent().hide();
        $("#log-added-contacts-title").hide();
    }

    if (importEntity.repeatedEmployees != null && importEntity.repeatedEmployees.length > 0) {

        $.each(importEntity.repeatedEmployees, function (index, employee) {

            var statusClass = employee.isActive ? 'active-pill' : 'inactive-pill';
            var statusText = employee.isActive ? 'Aktywny' : 'Nieaktywny';

            var newRow = `<tr>
                                    <td class="tiny-cell">${employee.name}</td>
                                    <td class="tiny-cell">${employee.surname}</td>
                                    <td class="small-cell">${employee.phoneNumber}</td>
                                    <td class="tiny-centered-cell"><span class="${statusClass}">${statusText}</span></td>
                                    </tr>`

            $("#log-detail-import-repeatedContacts-table tbody").append(newRow);
        });
    }
    else {
        $("#log-detail-import-repeatedContacts-table").parent().hide();
        $("#log-repeated-contacts-title").hide();
    }

    if (importEntity.invalidEmployees != null && importEntity.invalidEmployees.length > 0) {

        $.each(importEntity.invalidEmployees, function (index, employee) {

            var statusClass = employee.isActive ? 'active-pill' : 'inactive-pill';
            var statusText = employee.isActive ? 'Aktywny' : 'Nieaktywny';

            var newRow = `<tr>
                                        <td class="tiny-cell">${employee.name}</td>
                                        <td class="tiny-cell">${employee.surname}</td>
                                        <td class="small-cell">${employee.phoneNumber}</td>
                                        <td class="tiny-centered-cell"><span class="${statusClass}">${statusText}</span></td>
                                        </tr>`

            $("#log-detail-import-invalidContacts-table tbody").append(newRow);
        });
    }
    else {
        $("#log-detail-import-invalidContacts-table").parent().hide();
        $("#log-invalid-contacts-title").hide();
    }

    $(".logs-detail-label").css({
        "min-width": "148px",
        "max-width": "148px"
    });
}

function HandleGroupsTypeLog(groupEntity) {

    var groupDescription = groupEntity.groupDescription;

    var relatedObjects =
        `<span class="log-related-object-title">Grupa</span>
                <hr style="margin-top: 20px";/>
                <span class="form-subtitle">Informacje</span>
                <div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-GroupIdDetail">Id:</label>
                        <span class="details-span" id="logGroupAssign-GroupIdDetail">${groupEntity.groupId}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-GroupNameDetail">Nazwa:</label>
                        <span class="details-span" id="logGroupAssign-GroupNameDetail">${groupEntity.groupName}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-GroupDescriptionDetail">Opis:</label>
                        <span class="details-span" id="logGroupAssign-GroupDescriptionDetail">${(groupDescription == null || groupDescription == "") ? "Brak opisu" : groupDescription}</span>
                    </div>
                </div>`;

    $("#log-related-objects-data-form-group").append(relatedObjects);

    $(".logs-detail-label").css({
        "min-width": "148px",
        "max-width": "148px"
    });
}

function handleSMSGroupTypeLog(smsGroupObject) {
    var type = $("#logTypeDetail").text();

    console.log(smsGroupObject);

    var chosenGroupId = smsGroupObject.sms.chosenGroupId;
    var additionalPhoneNumbers = smsGroupObject.sms.additionalPhoneNumbers;
    var additionalInformation = smsGroupObject.sms.additionalInformation;
    var smsMessage = smsGroupObject.sms.settings["text"];

    var responseErrorServerSms = smsGroupObject.sms.serverResponse.error;
    var responseErrorSmsApi = smsGroupObject.sms.serverResponse;
    var responseSuccess = smsGroupObject.sms.serverResponse;

    var groupDescription = smsGroupObject.group.groupDescription;

    var relatedObjects =
        `<span class="log-related-object-title">Wiadomość SMS</span>
                <hr style="margin-top: 20px";/>
                <span class="form-subtitle">Informacje</span>
                    <div class="row mb-10" style="margin-top: 20px;">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-SenderDetail">Nazwa nadawcy:</label>
                            <span class="details-span" id="logSMSGroup-SenderDetail">${smsGroupObject.sms.settings["sender"] == null ? "Nie określono" : smsGroupObject.sms.settings["sender"]}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-ChosenGroupIdDetail">Id wybranej grupy:</label>
                            <span class="details-span" id="logSMSGroup-ChosenGroupIdDetail">${chosenGroupId == 0 ? "Nie wybrano grupy" : chosenGroupId}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-MessageDetail">Treść wiadomości:</label>
                                <span class="details-span" id="logSMSGroup-MessageDetail">${(smsMessage == "" || smsMessage == null) ? "Nie wpisano treści" : smsMessage}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-AdditionalPhoneNumbersDetail">Dodatkowe numery:</label>
                             <span class="details-span" id="logSMSGroup-AdditionalPhoneNumbersDetail">${additionalPhoneNumbers.length == 0 ? "Nie podano" : additionalPhoneNumbers.split(',').join(", ")}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-AdditionalInformationDetail">Komentarz:</label>
                            <span class="details-span" id="logSMSGroup-AdditionalInformationDetail">${(additionalInformation == null || additionalInformation == "") ? "Brak" : additionalInformation}</span>
                        </div>
                    </div>
                    <hr style="margin-top: 20px";/>
                    <span class="form-subtitle">Ustawienia</span>
                    <div class="row mb-10" style="margin-top: 20px;">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-TestDetail">Wiadomośc testowa:</label>
                            <span class="details-span" id="logSMSGroup-TestDetail">${smsGroupObject.sms.settings["test"] == "true" ? "Tak" : "Nie"}</span>
                        </div>
                    </div>
                    <div class="row mb-10" >
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-SpeedDetail">Kanał priorytetowy:</label>
                            <span class="details-span" id="logSMSGroup-SpeedDetail">${smsGroupObject.sms.settings["speed"] == "1" ? "Tak" : "Nie"}</span>
                        </div>
                    </div>
                    <hr style="margin-top: 20px";/>
                    <span class="form-subtitle">Odpowiedź servera</span>
                    `;

    if (type == "Błąd") {
        if (smsGroupObject.apiUsed == "ServerSms") {
            relatedObjects +=
                `
                    <div class="row mb-10" style="margin-top: 20px;">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-CodeDetail">Kod błędu:</label>
                                <span class="details-span" id="logSMSGroup-CodeDetail">${responseErrorServerSms.code}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-ErrorTypeDetail">Typ błędu:</label>
                                <span class="details-span" id="logSMSGroup-ErrorTypeDetail">${responseErrorServerSms.type}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-ErrorMessageDetail">Wiadomość błędu:</label>
                            <span class="details-span" id="logSMSGroup-ErrorMessageDetail">${responseErrorServerSms.message}</span>
                        </div>
                    </div>
                `;
        }
        else {
            relatedObjects +=
                `
                    <div class="row mb-10" style="margin-top: 20px;">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-CodeDetail">Kod błędu:</label>
                                <span class="details-span" id="logSMSGroup-CodeDetail">${responseErrorSmsApi.errorCode}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-ErrorMessageDetail">Wiadomość błędu:</label>
                                <span class="details-span" id="logSMSGroup-ErrorMessageDetail">${responseErrorSmsApi.errorMessage}</span>
                        </div>
                    </div>
                `;
        }

    }
    else {
        if (smsGroupObject.apiUsed == "ServerSms") {
            console.log(responseSuccess)
            relatedObjects +=
                `<div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSGroup-StatusDetail">Status:</label>
                        <span class="details-span" id="logSMSGroup-StatusDetail">${responseSuccess.success ? "Sukces" : "Błąd"}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSGroup-SentDetail">Wysłano:</label>
                        <span class="details-span" id="logSMSGroup-SentDetail">${responseSuccess.queued}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSGroup-UnsentDetail">Nie wysłano:</label>
                        <span class="details-span" id="logSMSGroup-UnsentDetail">${responseSuccess.unsent}</span>
                    </div>
                </div>
                `;
        }
        else {
            relatedObjects +=
                `<div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSGroup-StatusDetail">Status:</label>
                        <span class="details-span" id="logSMSGroup-StatusDetail">Sukces</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSGroup-SentDetail">Wysłano:</label>
                        <span class="details-span" id="logSMSGroup-SentDetail">${responseSuccess.details.filter(r => r.status == "QUEUE").length}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSGroup-UnsentDetail">Nie wysłano:</label>
                        <span class="details-span" id="logSMSGroup-UnsentDetail">${responseSuccess.details.filter(r => r.status != "QUEUE").length}</span>
                    </div>
                </div>
                `;
        }
    }
    relatedObjects +=
        `<hr style="margin-top: 20px;" />
                <span class="log-related-object-title">Grupa</span>
                <hr style="margin-top: 20px;" />
                <span class="form-subtitle">Informacje</span>
                <div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSGroup-GroupIdDetail">Id:</label>
                        <span class="details-span" id="logSMSGroup-GroupIdDetail">${smsGroupObject.group.groupId}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSGroup-GroupNameDetail">Nazwa:</label>
                        <span class="details-span" id="logSMSGroup-GroupNameDetail">${smsGroupObject.group.groupName}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSGroup-GroupDescriptionDetail">Opis:</label>
                        <span class="details-span" id="logSMSGroup-GroupDescriptionDetail">${(groupDescription == null || groupDescription == "") ? "Brak opisu" : groupDescription}</span>
                    </div>
                </div>
                <hr style="margin-top: 20px;"/>
                <span class="form-subtitle">Członkowie grupy</span>
                <div class="table-responsive" style="border: 2px solid #f0f1f1; margin-top:20px;">
                    <table class="log-group-members-list table table-borderless" id="log-detail-smsgroup-members-table">
                        <thead>
                            <tr>
                                <th class="tiny-cell">Imię</th>
                                <th class="tiny-cell">Nazwisko</th>
                                <th class="small-cell" style="width: 150px;">Numer telefonu</th>
                                <th class="tiny-centered-cell">Status</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                `;

    $("#log-related-objects-data-form-group").append(relatedObjects);

    $.each(smsGroupObject.group.members, function (index, employee) {

        var statusClass = employee.isActive ? 'active-pill' : 'inactive-pill';
        var statusText = employee.isActive ? 'Aktywny' : 'Nieaktywny';

        var newRow = `<tr>
                        <td class="tiny-cell">${employee.name}</td>
                        <td class="tiny-cell">${employee.surname}</td>
                        <td class="small-cell">${employee.phoneNumber}</td>
                        <td class="tiny-centered-cell"><span class="${statusClass}">${statusText}</span></td>
                        </tr>`

        $("#log-detail-smsgroup-members-table tbody").append(newRow);
    });

    $(".logs-detail-label").css({
        "min-width": "172px",
        "max-width": "172px"
    });
}

function handleSMSNoGroupTypeLog(smsNoGroupObject) {
    var type = $("#logTypeDetail").text();

    var chosenGroupId = smsNoGroupObject.sms.chosenGroupId;
    var additionalPhoneNumbers = smsNoGroupObject.sms.additionalPhoneNumbers;
    var additionalInformation = smsNoGroupObject.sms.additionalInformation;
    var smsMessage = smsNoGroupObject.sms.settings["text"];

    var responseErrorServerSms = smsNoGroupObject.sms.serverResponse.error;
    var responseErrorSmsApi = smsNoGroupObject.sms.serverResponse;
    var responseSuccess = smsNoGroupObject.sms.serverResponse;

    var relatedObjects =
        `
            <span class="log-related-object-title">Wiadomość SMS</span>
            <hr style="margin-top: 20px";/>
            <span class="form-subtitle">Informacje</span>
            <div class="row mb-10" style="margin-top: 20px;">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logSMSNoGroup-SenderDetail">Nazwa nadawcy:</label>
                    <span class="details-span" id="logSMSNoGroup-SenderDetail">${smsNoGroupObject.sms.settings.sender == null ? "Nie określono" : smsNoGroupObject.sms.settings.sender}</span>
                </div>
            </div>
            <div class="row mb-10">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logSMSNoGroup-ChosenGroupIdDetail">Id wybranej grupy:</label>
                    <span class="details-span" id="logSMSNoGroup-ChosenGroupIdDetail">${chosenGroupId == 0 ? "Nie wybrano grupy" : chosenGroupId}</span>
                </div>
            </div>
            <div class="row mb-10">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logSMSNoGroup-MessageDetail">Treść wiadomości:</label>
                        <span class="details-span" id="logSMSNoGroup-MessageDetail">${(smsMessage == "" || smsMessage == null) ? "Nie wpisano treści" : smsMessage}</span>
                </div>
            </div>
            <div class="row mb-10">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logSMSNoGroup-AdditionalPhoneNumbersDetail">Dodatkowe numery:</label>
                        <span class="details-span" id="logSMSNoGroup-AdditionalPhoneNumbersDetail">${additionalPhoneNumbers.length == 0 ? "Nie podano" : additionalPhoneNumbers.split(',').join(", ")}</span>
                </div>
            </div>
            <div class="row mb-10">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logSMSNoGroup-AdditionalInformationDetail">Komentarz:</label>
                    <span class="details-span" id="logSMSNoGroup-AdditionalInformationDetail">${(additionalInformation == null || additionalInformation == "") ? "Brak" : additionalInformation}</span>
                </div>
            </div>
            <hr style="margin-top: 20px";/>
            <span class="form-subtitle">Ustawienia</span>
            <div class="row mb-10" style="margin-top: 20px;">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logSMSNoGroup-TestDetail">Wiadomośc testowa:</label>
                    <span class="details-span" id="logSMSNoGroup-TestDetail">${smsNoGroupObject.sms.settings.test == "true" ? "Tak" : "Nie"}</span>
                </div>
            </div>
            <div class="row mb-10" >
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logSMSNoGroup-SpeedDetail">Kanał priorytetowy:</label>
                    <span class="details-span" id="logSMSNoGroup-SpeedDetail">${smsNoGroupObject.sms.settings.speed == "1" ? "Tak" : "Nie"}</span>
                </div>
            </div>
            <hr style="margin-top: 20px";/>
            <span class="form-subtitle">Odpowiedź servera</span>
            `;

    if (type == "Błąd") {
        if (smsNoGroupObject.apiUsed = "ServerSms") {
            relatedObjects +=
                `
                <div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSNoGroup-CodeDetail">Kod błędu:</label>
                        <span class="details-span" id="logSMSNoGroup-CodeDetail">${responseErrorServerSms.code}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSNoGroup-ErrorTypeDetail">Typ błędu:</label>
                        <span class="details-span" id="logSMSNoGroup-ErrorTypeDetail">${responseErrorServerSms.type}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSNoGroup-ErrorMessageDetail">Wiadomość błędu:</label>
                        <span class="details-span" id="logSMSNoGroup-ErrorMessageDetail">${responseErrorServerSms.message}</span>
                    </div>
                </div>
                `;
        }
        else {
            relatedObjects +=
                `
                <div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSNoGroup-CodeDetail">Kod błędu:</label>
                        <span class="details-span" id="logSMSNoGroup-CodeDetail">${responseErrorSmsApi.errorCode}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSNoGroup-ErrorMessageDetail">Wiadomość błędu:</label>
                        <span class="details-span" id="logSMSNoGroup-ErrorMessageDetail">${responseErrorSmsApi.errorMessage}</span>
                    </div>
                </div>
                `;
        }

    }
    else {
        if (smsNoGroupObject.apiUsed = "ServerSms") {
            relatedObjects +=
                `
                    <div class="row mb-10" style="margin-top: 20px;">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSNoGroup-StatusDetail">Status:</label>
                            <span class="details-span" id="logSMSNoGroup-StatusDetail">${responseSuccess.success ? "Sukces" : "Błąd"}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSNoGroup-SentDetail">Wysłano:</label>
                            <span class="details-span" id="logSMSNoGroup-SentDetail">${responseSuccess.queued}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSNoGroup-UnsentDetail">Nie wysłano:</label>
                            <span class="details-span" id="logSMSNoGroup-UnsentDetail">${responseSuccess.unsent}</span>
                        </div>
                    </div>
                `;
        }
        else {
            relatedObjects +=
                `
                    <div class="row mb-10" style="margin-top: 20px;">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSNoGroup-StatusDetail">Status:</label>
                            <span class="details-span" id="logSMSNoGroup-StatusDetail">Sukces</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSNoGroup-SentDetail">Wysłano:</label>
                            <span class="details-span" id="logSMSNoGroup-SentDetail">${responseSuccess.details.filter(r => r.status == "QUEUE").length}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSNoGroup-UnsentDetail">Nie wysłano:</label>
                            <span class="details-span" id="logSMSNoGroup-UnsentDetail">${responseSuccess.details.filter(r => r.status != "QUEUE").length}</span>
                        </div>
                    </div>
                `;
        }
    }

    $("#log-related-objects-data-form-group").append(relatedObjects);

    $(".logs-detail-label").css({
        "min-width": "172px",
        "max-width": "172px"
    });
}

function HandleContactTypeLog(contactEntity) {

    var email = contactEntity.email;
    var hqAddress = contactEntity.hqAddress;
    var postalNumber = contactEntity.postalNumber;
    var city = contactEntity.city;
    var department = contactEntity.department;


    var relatedObjects =
        `       <span class="log-related-object-title">Kontakt</span>
                    <hr style="margin-top: 20px";/>
                    <span class="form-subtitle">Dane personalne</span>
                    <div class="row mb-10" style="margin-top: 20px;">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeeIdDetail">Id:</label>
                            <span class="details-span" id="logContact-EmployeeIdDetail">${contactEntity.employeeId}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeeNameDetail">Imię:</label>
                            <span class="details-span" id="logContact-EmployeeNameDetail">${contactEntity.name}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeeSurnameDetail">Nazwisko:</label>
                            <span class="details-span" id="logContact-EmployeeSurnameDetail">${contactEntity.surname}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeeEmailDetail">Email:</label>
                            <span class="details-span" id="logDontact-EmployeeEmailDetail">${(email == null || email == "") ? "Brak danych" : email}</span>
                        </div>
                    </div>
                    <hr />
                    <span class="form-subtitle">Dane kontaktowe</span>
                    <div class="row mb-10" style="margin-top: 20px;">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeePhonenumberDetail">Numer telefonu:</label>
                            <span class="details-span" id="logContact-EmployeePhonenumberDetail">${contactEntity.phoneNumber}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeeHQAddressDetail">Adres miejsca pracy:</label>
                            <span class="details-span" id="logContact-EmployeeHQAddressDetail">${(hqAddress == null || hqAddress == "") ? "Brak danych" : hqAddress}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeePostalNumberDetail">Kod pocztowy:</label>
                            <span class="details-span" id="logContact-EmployeePostalNumberDetail">${(postalNumber == null || postalNumber == "") ? "Brak danych" : postalNumber}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeeCityDetail">Miasto:</label>
                            <span class="details-span" id="logContact-EmployeeCityDetail">${(city == null || city == "") ? "Brak danych" : city}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeeDepartmentDetail">Jednostka organizacyjna:</label>
                            <span class="details-span" id="logContact-EmployeeDepartmentDetail">${(department == null || department == "") ? "Brak danych" : department}</span>
                        </div>
                    </div>
                    <hr />
                        <div class="row mb-10" style="margin-top: 20px;">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeeStatusDetail">Status:</label>
                            <span class="details-span" id="logContact-EmployeeStatusDetail">${contactEntity.isActive ? "Aktywny" : "Nieaktywny"}</span>
                        </div>
                    </div>
                    `;

    $("#log-related-objects-data-form-group").append(relatedObjects);

    $("#logContact-EmployeeStatusDetail").text() == "Aktywny" ? $("#logContact-EmployeeStatusDetail").css('color', '#018707') : $("#logContact-EmployeeStatusDetail").css('color', '#df3b15');

    $(".logs-detail-label").css({
        "min-width": "172px",
        "max-width": "172px"
    });
}

function SanitizePolishSymbols(inputId) {

    const diacriticToAsciiMap = {
        'ą': 'a',
        'ć': 'c',
        'ę': 'e',
        'ł': 'l',
        'ń': 'n',
        'ó': 'o',
        'ś': 's',
        'ź': 'z',
        'ż': 'z',
        'Ą': 'A',
        'Ć': 'C',
        'Ę': 'E',
        'Ł': 'L',
        'Ń': 'N',
        'Ó': 'O',
        'Ś': 'S',
        'Ź': 'Z',
        'Ż': 'Z'
    };

    var input = document.getElementById(inputId);
    var inputValue = input.value;
    var sanitizedValue = inputValue.replace(/[ąćęłńóśźżĄĆĘŁŃÓŚŹŻ]/g, match => diacriticToAsciiMap[match] || match);
    input.value = sanitizedValue;
}

function DeleteTemplate(templateId) {

    $.ajax({
        url: '/Home/DeleteTemplate',
        type: 'GET',
        contentType: 'application/json',
        data: { id: templateId },
        success: function () {
            console.log("Successfully deleted template")
            FetchAllTemplatesAndPopulateTable();
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function CloseAlert() {
    $("#status-message-sms").hide();
}

function DeleteContact(contactId) {

    $.ajax({
        url: '/Home/DeleteContact',
        type: 'GET',
        contentType: 'application/json',
        data: { id: contactId },
        success: function () {
            console.log("Successfully deleted contact")
            FetchAllContactsAndPopulateTable();
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function DeleteGroup(groupId) {

    $.ajax({
        url: '/Home/DeleteGroup',
        type: 'GET',
        contentType: 'application/json',
        data: { id: groupId },
        success: function () {
            console.log("Successfully deleted group")
            FetchAllGroupsAndPopulateTable();
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function EditTemplate() {
    var templateId = $("#createOrEditButtonTemplates").attr('data-id');
    var templateName = $("#templateNameInput").val();
    var templateContent = $("#templateContentInput").val();
    var templateDescription = $("#templateDescriptionInput").val();
    $.ajax({
        url: '/Home/EditTemplate',
        type: 'GET',
        contentType: 'application/json',
        data: { id: templateId, name: templateName, description: templateDescription, content: templateContent },
        success: function (templateName) {
            console.log("Successfully edited template " + templateName)
            FetchAllTemplatesAndPopulateTable();

        },
        error: function (error) {
            console.error(error);
        }
    });
}

function EditContact() {
    var contactId = $("#createOrEditButtonContacts").attr('data-id');
    let name = $("#contactNameInput").val();
    let surname = $("#contactSurnameInput").val();
    let email = $("#contactEmail").val();
    let phoneNumber = $("#contactPhoneNumber").val();
    let address = $("#contactHQAddress").val();
    let zip = $("#ContactPostalNumber").val();
    let city = $("#ContactCity").val();
    let department = $("#contactDepartment").val();
    let isActive = document.querySelector('input[name="isActive?"]:checked').value;
    let assignedRoles = null;
    let assignedGroups = null;

    $.ajax({
        url: '/Home/EditContact',
        type: 'GET',
        contentType: 'application/json',
        data: { contactId: contactId, contactName: name, contactSurname: surname, email: email, phone: phoneNumber, address: address, zip: zip, city: city, department: department, isActive: isActive },
        success: function (contactName) {
            console.log("Successfully edited contact " + contactName)
            FetchAllContactsAndPopulateTable();

        },
        error: function (error) {
            console.error(error);
        }
    });
}

function EditGroup() {
    var groupId = $("#createOrEditButtonGroups").attr('data-id');
    var groupName = $("#groupNameInput").val();
    var groupDescription = $("#groupDescriptionInput").val();
    $.ajax({
        url: '/Home/EditGroup',
        type: 'GET',
        contentType: 'application/json',
        data: { id: groupId, name: groupName, description: groupDescription },
        success: function (groupName) {
            console.log("Successfully edited group " + groupName)
            FetchAllGroupsAndPopulateTable();
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function FetchAllTemplatesAndPopulateTable() {
    $.ajax({
        url: '/Home/FetchAllTemplates',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfTemplates) {
            console.log("Successfully retrieved templates")
            $('.template-list tbody').empty();

            $.each(listOfTemplates, function (index, item) {
                var description = item.templateDescription;
                if (description == null) {
                    description = "Brak opisu"
                }

                var newRow = `<tr>
                            <td class="small-cell template-name">${item.templateName}</td>
                            <td class="medium-cell template-description">${description}</td>
                            <td class="big-cell template-content">${item.templateContent}</td>
                            <td class="centered-cell">
                            <a href="#details-${item.templateId}" class="icon-list template-details"><img src="/icons/view-doc.png" title="Szczegóły"/></a>
                            <a href="#edit-${item.templateId}" class="icon-list template-edit"><img src="/icons/edit.png" title="Edytuj"/></a>
                            <a href="#delete-${item.templateId}" class="icon-list template-delete"><img src="/icons/trash.png" title="Usuń"/></a>
                            </td>
                            </tr>`;


                $('.template-list tbody').append(newRow);
            });

        },
        error: function (error) {
            console.error(error);
        }
    })
}

function FetchAllContactsAndPopulateTable() {
    $.ajax({
        url: '/Home/FetchAllContacts',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfContacts) {
            console.log("Successfully retrieved contacts")
            $('.contacts-list tbody').empty();

            $.each(listOfContacts, function (index, item) {

                var groupNames = item.employeeGroupNames;
                var email = item.email;

                groupNames = (groupNames == null || groupNames.length == 0) ? "Nie przypisano" : groupNames.join(", ")
                email = (email == null || email == "") ? "Brak danych" : email
                var isActiveRow = item.isActive ? '<td class="centered-cell"><span class="active-pill">Aktywny<span></td>' : '<td class="centered-cell"><span class="inactive-pill">Nieaktywny<span></td>'

                var newRow = `<tr>
                            <td class="tiny-cell contact-name">${item.name}</td>
                            <td class="tiny-cell contact-surname">${item.surname}</td>
                            <td class="small-cell">${email}</td>
                            <td class="small-cell">${item.phoneNumber}</td>
                            ${isActiveRow}
                            <td class="centered-cell">${groupNames}</td>
                            <td class="centered-cell">
                            <a href="#details-${item.employeeId}" class="icon-list contact-details"><img src="/icons/view-doc.png" title="Szczegóły"/></a>
                            <a href="#edit-${item.employeeId}" class="icon-list contact-edit"><img src="/icons/edit.png" title="Edytuj"/></a>
                            <a href="#delete-${item.employeeId}" class="icon-list contact-delete"><img src="/icons/trash.png" title="Usuń"/></a>
                            </td>
                            </tr>`;


                $('.contacts-list tbody').append(newRow);
            });

        },
        error: function (error) {
            console.error(error);
        }
    })
}

function FetchAllGroupsAndPopulateTable() {
    $.ajax({
        url: '/Home/FetchAllGroups',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfGroups) {
            console.log("Successfully retrieved groups")
            $('.group-list tbody').empty();

            $.each(listOfGroups, function (index, item) {
                var groupDescription = item.groupDescription;

                groupDescription = groupDescription == null ? "Brak opisu" : groupDescription;

                var newRow = `<tr>
                            <td class="small-cell group-name">${item.groupName}</td>
                            <td class="big-cell group-description">${groupDescription}</td>
                            <td class="tiny-centered-cell">${item.membersIds.length}</td>
                                <td class="centered-cell"" style="min-width: 205px;">
                            <a href="#assign-${item.groupId}" class="icon-list group-assign"><img src="/icons/assign-users.png" title="Przypisz użytkowników"/></a>
                            <a href="#details-${item.groupId}" class="icon-list group-details"><img src="/icons/view-doc.png" title="Szczegóły"/></a>
                            <a href="#edit-${item.groupId}" class="icon-list group-edit"><img src="/icons/edit.png" title="Edytuj"/></a>
                            <a href="#delete-${item.groupId}" class="icon-list group-delete"><img src="/icons/trash.png" title="Usuń"/></a>
                            </td>
                            </tr>`;


                $('.group-list tbody').append(newRow);
            });

        },
        error: function (error) {
            console.error(error);
        }
    })
}

function FetchAllLogsAndPopulateTable() {
    $.ajax({
        url: '/Home/FetchAllLogs',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfLogs) {
            console.log("Successfully retrieved logs")
            $('#log-table tbody').empty();

            $.each(listOfLogs, function (index, item) {

                var logTypeRow = item.logType == "Błąd" ? `<td class="tiny-centered-cell"><span class="error-pill">${item.logType}</span></td>` : `<td class="tiny-centered-cell"><span class="info-pill">${item.logType}</span></td>`

                var newRow = `<tr>
                            ${logTypeRow}
                            <td class="tiny-cell">${item.logSource}</td>
                            <td class="big-cell">${item.logMessage}</td>
                            <td class="centered-cell" style="min-width:105px;">${new Date(item.logCreationDate).toLocaleString('en-GB')}</td>
                            <td class="tiny-centered-cell">
                            <a href="#details-${item.logId}" class="icon-list log-details"><img src="/icons/view-doc.png" title="Szczegóły"/></a>
                            </td>
                            </tr>`;


                $('#log-table tbody').append(newRow);
            });

        },
        error: function (error) {
            console.error(error);
        }
    })
}

function PopulateTableForChooseGroupForSMS() {
    $.ajax({
        url: '/Home/FetchAllValidGroups',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfGroups) {
            console.log("Successfully retrieved groups")
            $('#choose-group-for-sms-table tbody').empty();

            $.each(listOfGroups, function (index, item) {

                var description = item.groupDescription == null ? "Brak opisu" : item.groupDescription

                var newRow = `<tr>
                            <td class="small-cell">${item.groupName}</td>
                            <td class="big-cell">${description}</td>
                            <td class="tiny-centered-cell">${item.membersIds.length}</td>
                            <td class="tiny-centered-cell">
                            <a href="#pick-${item.groupId}" class="icon-list group-pick-me"><img src="/icons/pick-me.png" title="Wybierz grupę"/></a>
                            </td>
                            </tr>`;


                $('#choose-group-for-sms-table tbody').append(newRow);
            });
        },
        error: function (error) {
            console.error(error);
        }
    })
}

function PopulateTableForChooseTemplateForSMS() {
    $.ajax({
        url: '/Home/FetchAllTemplates',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfTemplates) {
            console.log("Successfully retrieved templates")
            $('#choose-template-for-sms-table tbody').empty();

            $.each(listOfTemplates, function (index, item) {

                var templateDescription = item.templateDescription == null ? "Brak opisu" : item.templateDescription

                var newRow = `<tr>
                            <td class="small-cell">${item.templateName}</td>
                            <td class="medium-cell">${templateDescription}</td>
                            <td class="big-cell">${item.templateContent}</td>
                            <td class="tiny-centered-cell">
                            <a href="#pick-${item.templateId}" class="icon-list template-pick-me"><img src="/icons/pick-me.png" title="Wybierz szablon"/></a>
                            </td>
                            </tr>`;


                $('#choose-template-for-sms-table tbody').append(newRow);
            });

        },
        error: function (error) {
            console.error(error);
        }
    });
}

function PopulateTablesForAssigningUsersToGroups(groupId) {
    $.ajax({
        url: '/Home/GetGroupById',
        type: 'GET',
        data: { id: groupId },

        contentType: 'application/json',
        success: function (groupEntity) {

            var groupDescription = groupEntity.groupDescription == null ? "Brak opisu" : groupEntity.groupDescription;
            $("#group-assign-chosen-group-table tbody").empty();

            var newRow = `<tr>
                        <td class="small-cell">${groupEntity.groupName}</td>
                        <td class="big-cell">${groupDescription}</td>
                        <td class="tiny-centered-cell">${groupEntity.membersIds.length}</td>
                        </tr>`;

            $('#group-assign-chosen-group-table').append(newRow);

            $.ajax({
                url: '/Home/FetchAllContacts',
                type: 'GET',
                contentType: 'application/json',
                success: function (listOfContacts) {

                    $('#group-assign-contact-list-table tbody').empty();

                    $.each(listOfContacts, function (index, item) {

                        var groupNames = item.employeeGroupNames;

                        groupNames = (groupNames == null || groupNames.length === 0) ? "Nie przypisano" : groupNames.join(", ")
                        var isActiveRow = item.isActive ? '<td class="tiny-centered-cell"><span class="active-pill">Aktywny<span></td>' : '<td class="tiny-centered-cell"><span class="inactive-pill">Nieaktywny<span></td>'

                        var newRow = `<tr>
                                    <td class="tiny-cell">${item.name}</td>
                                    <td class="tiny-cell">${item.surname}</td>
                                    <td class="tiny-cell">${item.phoneNumber}</td>
                                    ${isActiveRow}
                                    <td class="centered-cell">${groupNames}</td>
                                    <td class="tiny-centered-cell">`

                        if (groupEntity.membersIds.includes(item.employeeId)) {

                            newRow += `<a href="#unassign-${item.employeeId}-${groupEntity.groupId}" class="icon-list contact-unassign"><img src="/icons/unassign-user.png" title="Wypisz z grupy"/></a>
                                        </td>
                                        </tr>`;
                        }
                        else {
                            newRow += `<a href="#assign-${item.employeeId}-${groupEntity.groupId}" class="icon-list contact-assign"><img src="/icons/assign-user.png" title="Dopisz do grupy" /></a>
                                        </td>
                                        </tr>`;
                        }

                        $('#group-assign-contact-list-table tbody').append(newRow);
                    });
                },
                error: function (error) {
                    console.error(error);
                }
            });

            $(".groups-list-container").hide();
            $(".groups-options-container-assign").show();
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function CreateNewTemplate() {
    let name = $("#templateNameInput").val();
    let description = $("#templateDescriptionInput").val();
    let content = $("#templateContentInput").val();

    $.ajax({
        url: '/Home/CreateNewTemplate',
        type: 'GET',
        contentType: 'application/json',
        data: { templateName: name, templateDescription: description, templateContent: content },
        success: function (templateEnitity) {
            FetchAllTemplatesAndPopulateTable();
            console.log("Successfully added new template")
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function CreateNewContact() {
    let name = $("#contactNameInput").val();
    let surname = $("#contactSurnameInput").val();
    let email = $("#contactEmail").val();
    let phoneNumber = $("#contactPhoneNumber").val();
    let address = $("#contactHQAddress").val();
    let zip = $("#ContactPostalNumber").val();
    let city = $("#ContactCity").val();
    let department = $("#contactDepartment").val();
    let isActive = document.querySelector('input[name="isActive?"]:checked').value;

    $.ajax({
        url: '/Home/CreateNewContact',
        type: 'GET',
        contentType: 'application/json',
        data: { contactName: name, contactSurname: surname, email: email, phone: phoneNumber, address: address, zip: zip, city: city, department: department, isActive: isActive },
        success: function (contactEnitityName) {
            FetchAllContactsAndPopulateTable();
            console.log("Successfully added new contact: " + contactEnitityName)
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function CreateNewGroup() {
    let name = $("#groupNameInput").val();
    let description = $("#groupDescriptionInput").val();

    $.ajax({
        url: '/Home/CreateNewGroup',
        type: 'GET',
        contentType: 'application/json',
        data: { groupName: name, groupDescription: description },
        success: function (groupName) {
            FetchAllGroupsAndPopulateTable();
            console.log("Successfully added new template: " + groupName)
        },
        error: function (error) {
            console.error(error);
        }
    });
}

function TriggerFileInput() {
    $("#import-file-input").click();
}

function OpenCreateTemplateWindow() {
    const messageEle = document.getElementById('templateContentInput');
    document.getElementById('symbolCounterAdd').innerHTML = messageEle.value.length + "/" + messageEle.getAttribute('maxlength');
    $("#symbolCounterAdd").innerHTML = '25';
    $("#template-input-window-title").html("Nowy szablon")
    $(".template-options-form").attr('id', 'create-template-form');
    $("#createOrEditButtonTemplates").html("Utwórz");
    $(".templates-list-container").hide();
    $(".templates-options-container").show();
}

function OpenCreateContactWindow() {
    document.getElementById("isActiveYes").checked = true;
    $("#contact-input-window-title").html("Nowy kontakt")
    $(".contact-options-form").attr('id', 'create-contact-form');
    $("#createOrEditButtonContacts").html("Utwórz");
    $(".contacts-list-container").hide();
    $(".contacts-options-container").show();
}

function OpenCreateGroupWindow() {
    const messageEle = document.getElementById('groupDescriptionInput');
    document.getElementById('symbolCounterAddGroups').innerHTML = messageEle.value.length + "/" + messageEle.getAttribute('maxlength');
    $("#group-input-window-title").html("Nowa grupa")
    $(".group-options-form").attr('id', 'create-group-form');
    $("#createOrEditButtonGroups").html("Utwórz");
    $(".groups-list-container").hide();
    $(".groups-options-container").show();
}

function OpenChooseGroupForSMS() {
    PopulateTableForChooseGroupForSMS();
    $(".sms-container").hide();
    $(".choose-group-for-sms-container").show();
}

function OpenChooseTemplateForSMS() {
    PopulateTableForChooseTemplateForSMS();
    $(".sms-container").hide();
    $(".choose-template-for-sms-container").show();
}

function FinishBrowsingImportReport() {
    FetchAllContactsAndPopulateTable();
    $(".import-report-container").hide();
    $("#import-repeated-contacts-container").show();
    $("#import-invalid-contacts-container").show();
    $("#import-added-contacts-container").show();
    $('#repeated-contacts-table tbody').empty();
    $('#invalid-contacts-table tbody').empty();
    $('#added-contacts-table tbody').empty();
    $(".contacts-list-container").show();
}

function GoBackToSMSWindow() {
    $(".choose-group-for-sms-container").hide();
    $(".choose-template-for-sms-container").hide();
    $(".sms-container").show();
}

function GoBackToTemplateList() {
    $("#createOrEditButtonTemplates").removeAttr("data-id");
    $(".templates-list-container").show();
    $(".templates-options-container").hide();
    $(".templates-details-container").hide();
    $(".template-options-form").removeAttr('id');
    $(".template-options-form :input").val('');
    $(".templates-details-container .details-span").text("");
}

function GoBackToContactList() {
    $(".contact-options-form").removeAttr('id');
    $(".contact-options-form :input[type='text']").val('');
    $(".contact-options-form :input[type='email']").val('');
    $(".contact-options-form :input[type='tel']").val('');
    $(".contacts-details-container .details-span").text('');

    $(".contacts-details-container").hide();
    $(".contacts-options-container").hide();
    $(".contacts-list-container").show();
}

function GoBackToGroupList() {
    $(".group-options-form").removeAttr('id');
    $(".group-options-form :input").val('');
    $(".groups-details-container .details-span").text("");
    $(".groups-options-container").hide();
    $(".groups-options-container-assign").hide();
    $(".groups-details-container").hide();
    $(".groups-list-container").show();
}

function GoBackToLogsList() {
    $(".logs-details-container").hide();
    $(".logs-list-container").show();
    $(".details-span").text("");
    $("#log-related-objects-data-form-group").empty();
    $("#log-details-window").css("max-width", "600px")
}

function GoBackToGroupListFromAssign() {
    FetchAllGroupsAndPopulateTable();
    $(".groups-options-container-assign").hide();
    $(".groups-list-container").show();
}
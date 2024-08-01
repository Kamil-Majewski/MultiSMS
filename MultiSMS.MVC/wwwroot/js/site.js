function debounce(func, timeout = 300) {
    let timer;
    return (...args) => {
        if (!timer) {
            func.apply(this, args);
        }
        clearTimeout(timer);
        timer = setTimeout(() => {
            timer = undefined;
        }, timeout);
    };
}

function showEyeIconOrResetPasswordField(fieldEl, eyeEl) {

    $(fieldEl).on('input', function () {
        if ($(fieldEl).val().length > 0) {
            $(eyeEl).css('display', 'block')
        }
        else {
            if ($(fieldEl).attr('type') == "text") {
                $(eyeEl).click().hide();
            }
            else {
                $(eyeEl).hide();
            }
        }
    });
}
function togglePasswordVisibility(passwordInputEl, eyeIconEl) {

    $(eyeIconEl).click(function () {
        if (passwordInputEl.type === 'password') {
            passwordInputEl.type = 'text';
            eyeIconEl.src = window.location.origin + "/icons/eye.png";
        } else {
            passwordInputEl.type = 'password';
            eyeIconEl.src = window.location.origin + "/icons/closed-eye.png";
        }
    });
}

function CalculateHeaderWidthAndToolbarHeight() {

    const sidebar = document.getElementById('toolbar');
    const header = document.getElementById('main-header');
    const renderedContent = document.getElementById('content-render');

    header.style.width = `${renderedContent.offsetWidth + 50}px`;

    renderedContent.style.height = 'auto';
    sidebar.style.height = `auto`;

    if (sidebar.offsetHeight > 0) {
        if (renderedContent.offsetHeight > sidebar.offsetHeight) {
            sidebar.style.height = `${renderedContent.offsetHeight + 1}px`;
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

function OnSubmitFilterTemplatesTable(formIdentifiaction, searchBarIdentification, tableIdentification) {
    $(formIdentifiaction).on('submit', function (e) {
        e.preventDefault();

        const templatePageCounter = $("#template-page-counter");
        const templateNextButtonContainer = $("#template-next-button-container");
        const templatePreviousButtonContainer = $("#template-previous-button-container");

        templatePageCounter.hide();
        templateNextButtonContainer.hide();
        templatePreviousButtonContainer.hide();

        var searchPhrase = $(searchBarIdentification).val().toLowerCase();

        if (searchPhrase == "") {

            var firstId = $(tableIdentification).attr("first-id");

            PaginateTemplatesAndPopulateTable(firstId, null, 11, null);

            templateNextButtonContainer.show();
            templatePreviousButtonContainer.show();
            return;
        }

        $.ajax({
            url: `/Home/GetTemplatesBySeachPhrase`,
            type: 'GET',
            contentType: 'application/json',
            data: { searchPhrase },
            success: function (listOfTemplates) {
                const templateListBody = $(`${tableIdentification} tbody`);

                templateListBody.empty();

                listOfTemplates.forEach(template => {
                    var newRow = `
                        <tr>
                            <td class="small-cell template-name">${template.templateName}</td>
                            <td class="medium-cell template-description">${template.templateDescription}</td>
                            <td class="big-cell template-content">${template.templateContent}</td>
                            <td class="centered-cell">
                                <a href="#details-${template.templateId}" class="icon-list template-details">
                                    <img src="/icons/view-doc.png" title="Szczegóły">
                                </a>
                                <a href="#edit-${template.templateId}" class="icon-list template-edit">
                                    <img src="/icons/edit.png" title="Edytuj">
                                </a>
                                <a href="#delete-${template.templateId}" class="icon-list template-delete">
                                    <img src="/icons/trash.png" title="Usuń">
                                </a>
                            </td>
                        </tr>
                    `;

                    templateListBody.append(newRow);
                });
            },
            error: function (error) {
                console.error(error.responseText);
            }
        })
    });
}

function OnSubmitFilterContactsTable(formIdentifiaction, searchBarIdentification, tableIdentification) {
    $(formIdentifiaction).on('submit', function (e) {
        e.preventDefault();

        const contactPageCounter = $("#contact-page-counter");
        const contactNextButtonContainer = $("#contact-next-button-container");
        const contactPreviousButtonContainer = $("#contact-previous-button-container");

        contactPageCounter.hide();
        contactNextButtonContainer.hide();
        contactPreviousButtonContainer.hide();

        var searchPhrase = $(searchBarIdentification).val().toLowerCase();

        if (searchPhrase == "") {

            var firstId = $(tableIdentification).attr("first-id");

            PaginateContactsAndPopulateTable(firstId, null, 11, null);

            contactNextButtonContainer.show();
            contactPreviousButtonContainer.show();

            return;
        }

        $.ajax({
            url: `/Home/GetContactsBySearchPhrase`,
            type: 'GET',
            contentType: 'application/json',
            data: { searchPhrase },
            success: function (listOfContacts) {
                const contactListBody = $(`${tableIdentification} tbody`);

                contactListBody.empty();
                 
                listOfContacts.forEach(contact => {
                    var newRow = `
                        <tr>
                            <td class="tiny-cell contact-name">${contact.name}</td>
                            <td class="tiny-cell contact-surname">${contact.surname}</td>
                            <td class="small-cell contact-email">${contact.email || "Brak danych"}</td>
                            <td class="small-cell contact-phone">${contact.phoneNumber}</td>
                            <td class="centered-cell contact-activity">
                                <span class="${contact.isActive ? "active-pill" : "inactive-pill"}">${contact.isActive ? "Aktywny" : "Nieaktywny"}</span>
                            </td>
                            <td class="centered-cell contact-groups">${(contact.employeeGroupNames == null || contact.employeeGroupNames.length == 0) ? "Nie przypisano" : contact.employeeGroupNames.join(", ")}</td>
                            <td class="centered-cell">
                                <a href="#details-${contact.employeeId}" class="icon-list contact-details">
                                    <img src="/icons/view-doc.png" title="Szczegóły">
                                </a>
                                <a href="#edit-${contact.employeeId}" class="icon-list contact-edit">
                                    <img src="/icons/edit.png" title="Edytuj">
                                </a>
                                <a href="#delete-${contact.employeeId}" class="icon-list contact-delete">
                                    <img src="/icons/trash.png" title="Usuń">
                                </a>
                            </td>
                        </tr>
                    `;

                    contactListBody.append(newRow);
                })
            },
            error: function (error) {
                console.error(error.responseText);
            }
        });
    });
}

function OnSubmitFilterAssignContactsTable(formIdentifiaction, searchBarIdentification, tableIdentification) {
    $(formIdentifiaction).on('submit', function (e) {
        e.preventDefault();

        const contactPageCounter = $("#contact-assign-page-counter");
        const contactNextButtonContainer = $("#contacts-assign-list-next-page-button");
        const contactPreviousButtonContainer = $("#contact-assign-previous-button-container");

        contactPageCounter.hide();
        contactNextButtonContainer.hide();
        contactPreviousButtonContainer.hide();

        var searchPhrase = $(searchBarIdentification).val().toLowerCase();

        if (searchPhrase == "") {

            var firstId = $(tableIdentification).attr("first-id");

            PaginateAssignContactsAndPopulateTable(firstId, null, 7, null);

            contactNextButtonContainer.show();
            contactPreviousButtonContainer.show();

            return;
        }

        $.ajax({
            url: `/Home/GetContactsBySearchPhrase`,
            type: 'GET',
            contentType: 'application/json',
            data: { searchPhrase },
            success: function (listOfContacts) {
                const contactListBody = $(`${tableIdentification} tbody`);

                contactListBody.empty();

                listOfContacts.forEach(contact => {
                    var newRow = `
                        <tr>
                            <td class="tiny-cell contact-name">${contact.name}</td>
                            <td class="tiny-cell contact-surname">${contact.surname}</td>
                            <td class="tiny-cell contact-phone">${contact.phoneNumber}</td>
                            <td class="tiny-centered-cell contact-activity">
                                <span class="${contact.isActive ? "active-pill" : "inactive-pill"}">${contact.isActive ? "Aktywny" : "Nieaktywny"}</span>
                            </td>
                            <td class="centered-cell contact-groups">${(contact.employeeGroupNames == null || contact.employeeGroupNames.length == 0) ? "Nie przypisano" : contact.employeeGroupNames.join(", ")}</td>         
                            <td class="tiny-centered-cell">
                    `;

                    if (groupAssignMembersIds.includes(contact.employeeId)) {

                        newRow += `<a href="#unassign-${contact.employeeId}-${grouAssignGroupId}" class="icon-list contact-unassign"><img src="/icons/unassign-user.png" title="Wypisz z grupy"/></a>
                                        </td>
                                        </tr>`;
                    }
                    else {
                        newRow += `<a href="#assign-${contact.employeeId}-${grouAssignGroupId}" class="icon-list contact-assign"><img src="/icons/assign-user.png" title="Dopisz do grupy" /></a>
                                        </td>
                                        </tr>`;
                    }

                    contactListBody.append(newRow);
                });
            },
            error: function (error) {
                console.error(error.responseText);
            }
        });
    });
}

function OnSubmitFilterAssignGroupsTable(formIdentifiaction, searchBarIdentification, tableIdentification) {
    $(formIdentifiaction).on('submit', function (e) {
        e.preventDefault();

        const groupPageCounter = $("#group-assign-page-counter");
        const groupNextButtonContainer = $("#groups-assign-list-next-page-button");
        const groupPreviousButtonContainer = $("#groups-assign-list-previous-page-button");

        groupPageCounter.hide();
        groupNextButtonContainer.hide();
        groupPreviousButtonContainer.hide();

        var searchPhrase = $(searchBarIdentification).val().toLowerCase();

        if (searchPhrase == "") {

            var firstId = $(tableIdentification).attr("first-id");

            PaginateAssignGroupsAndPopulateTable(firstId, null, 7, null);

            groupNextButtonContainer.show();
            groupPreviousButtonContainer.show();

            return;
        }

        $.ajax({
            url: `/Home/GetGroupsBySearchPhrase`,
            type: 'GET',
            contentType: 'application/json',
            data: { searchPhrase },
            success: function (listOfGroups) {
                const groupListBody = $(`${tableIdentification} tbody`);

                groupListBody.empty();

                listOfGroups.forEach(group => {
                    var newRow = `
                        <tr>
                            <td class="small-cell group-name">${group.groupName}</td>
                            <td class="big-cell group-description">${group.groupDescription || "Brak opisu"}</td>
                            <td class="tiny-centered-cell group-members">${group.membersIds.length}</td>
                            <td class="tiny-centered-cell">
                    `;

                    if (group.membersIds.includes(parseInt(groupAssignContactId))) {

                        newRow += `<a href="#unassign-${groupAssignContactId}-${group.groupId}" class="icon-list contact-group-unassign"><img src="/icons/unassign-user.png" title="Wypisz z grupy"/></a>
                                        </td>
                                        </tr>`;
                    }
                    else {
                        newRow += `<a href="#assign-${groupAssignContactId}-${group.groupId}" class="icon-list contact-group-assign"><img src="/icons/assign-user.png" title="Dopisz do grupy"/></a>
                                        </td>
                                        </tr>`;
                    }

                    groupListBody.append(newRow);
                });
            },
            error: function (error) {
                console.error(error.responseText);
            }
        });
    });
}

function OnSubmitFilterGroupsTable(formIdentifiaction, searchBarIdentification, tableIdentification) {
    $(formIdentifiaction).on('submit', function (e) {
        e.preventDefault();

        const groupPageCounter = $("#group-page-counter");
        const groupNextButtonContainer = $("#group-next-button-container");
        const groupPreviousButtonContainer = $("#group-previous-button-container");

        groupPageCounter.hide();
        groupNextButtonContainer.hide();
        groupPreviousButtonContainer.hide();

        var searchPhrase = $(searchBarIdentification).val().toLowerCase();

        if (searchPhrase == "") {

            var firstId = $(tableIdentification).attr("first-id");

            PaginateGroupsAndPopulateTable(firstId, null, 11, null);

            groupNextButtonContainer.show();
            groupPreviousButtonContainer.show();

            return;
        }

        $.ajax({
            url: `/Home/GetGroupsBySearchPhrase`,
            type: 'GET',
            contentType: 'application/json',
            data: { searchPhrase },
            success: function (listOfGroups) {
                const groupListBody = $(`${tableIdentification} tbody`);

                groupListBody.empty();

                listOfGroups.forEach(group => {
                    var newRow = `
                        <tr>
                            <td class="small-cell group-name">${group.groupName}</td>
                            <td class="big-cell group-description">${group.groupDescription}</td>
                            <td class="tiny-centered-cell contact-email">${group.membersIds.length}</td>
                            <td class="centered-cell" style="min-width:205px !important;">
                                <a href="#assign-${group.groupId}" class="icon-list group-assign">
                                    <img src="/icons/assign-users.png" title="Przypisz użytkowników">
                                </a>
                                <a href="#details-${group.groupId}" class="icon-list group-details">
                                    <img src="/icons/view-doc.png" title="Szczegóły">
                                </a>
                                <a href="#edit-${group.groupId}" class="icon-list group-edit">
                                    <img src="/icons/edit.png" title="Edytuj">
                                </a>
                                <a href="#delete-${group.groupId}" class="icon-list group-delete">
                                    <img src="/icons/trash.png" title="Usuń">
                                </a>
                            </td>
                        </tr>
                        
                    `;
                    groupListBody.append(newRow);
                })
            },
            error: function (error) {
                console.error(error.responseText);
            }
        });
    });
}

function OnSubmitFilterLogsTable(formIdentifiaction, searchBarIdentification, tableIdentification) {
    $(formIdentifiaction).on('submit', function (e) {
        e.preventDefault();

        const logPageCounter = $("#log-page-counter");
        const logNextButtonContainer = $("#log-next-button-container");
        const logPreviousButtonContainer = $("#log-previous-button-container");

        logPageCounter.hide();
        logNextButtonContainer.hide();
        logPreviousButtonContainer.hide();

        var searchPhrase = $(searchBarIdentification).val().toLowerCase();

        if (searchPhrase == "") {

            var firstId = $(tableIdentification).attr("first-id");

            PaginateLogsAndPopulateTable(firstId, null, 11, null);

            logNextButtonContainer.show();
            logPreviousButtonContainer.show();

            return;
        }

        $.ajax({
            url: `/Home/GetLogsBySearchPhrase`,
            type: 'GET',
            contentType: 'application/json',
            data: { searchPhrase },
            success: function (listOfLogs) {
                const logListBody = $(`${tableIdentification} tbody`);

                logListBody.empty();

                listOfLogs.forEach(log => {

                    var logTypeRow = log.logType == "Błąd" ? `<td class="tiny-centered-cell"><span class="error-pill">${log.logType}</span></td>` : `<td class="tiny-centered-cell"><span class="info-pill">${log.logType}</span></td>`;

                    var newRow = `
                        <tr>
                            
                            ${logTypeRow}
                            <td class="tiny-cell">${log.logSource}</td>
                            <td class="big-cell">${log.logMessage}</td>
                            <td class="centered-cell" style="min-width:105px;">${new Date(log.logCreationDate).toLocaleString('pl-PL')}</td>
                            <td class="tiny-centered-cell">
                                <a href="#details-${log.logId}" class="icon-list log-details"><img src="/icons/view-doc.png" title="Szczegóły"/></a>
                            </td>
                        </tr>                        
                    `;
                    logListBody.append(newRow);
                })
            },
            error: function (error) {
                console.error(error.responseText);
            }
        });
    });
}

function SendSMS(text, chosenGroupId, chosenGroupName, additionalPhoneNumbers, additionalInfo) {
    function findUniqueErrors(value, index, array) {
        return array.indexOf(value) === index;
    }

    $.ajax({
        url: '/SmsApi/SendSmsMessage',
        type: 'GET',
        contentType: 'application/json',
        data: { text: text, chosenGroupId: chosenGroupId, chosenGroupName: chosenGroupName, additionalPhoneNumbers: additionalPhoneNumbers, additionalInfo: additionalInfo },
        success: function (response) {
            switch (response.status) {
                case "failed":

                    console.error(`Operation failed, server responded with code ${response.code} - ${response.message}`);
                    $("#status-message-sms").addClass("failed-status");
                    $("#status-message-sms").html(`Operacja zakończyła się niepowodzeniem, błąd ${response.code} - ${response.message} <button type='button' class='btn-close text-dark' aria-label='Close' onclick='CloseAlert()'></button>`);

                    break;
                case "success" || "Multiple-Success":

                    console.log(`Operation successful, messages queued: ${response.queued}, messages unsent: ${response.unsent}`);
                    $("#status-message-sms").addClass("success-status");
                    $("#status-message-sms").html(`Wiadomość została wysłana (zakolejkowane: ${response.queued}, niewysłane: ${response.unsent}) <button type='button' class='btn-close text-dark' onclick='CloseAlert()' aria-label='Close'></button>`);
                    $("#send-sms-form :input").val('');
                    document.getElementById('sms-text-symbol-counter').innerHTML = "";
                    $("#GroupOfContacts").removeAttr("data-id");
                    $(".input-icons").hide();

                    break;
                case "Multiple-Failure":
                    console.log(`Operation failed, errors detected!`);

                    const uniqueErrors = response.errors.filter(findUniqueErrors);

                    var newList = `<ul style="margin-top: 8px; margin-bottom: 0;">`;
                    uniqueErrors.forEach((error) => {
                        newList += `<li>${error}</li>`;
                    })
                    newList += "</ul>"

                    $("#status-message-sms").addClass("failed-status");
                    $("#status-message-sms").html(`Operacja zakończyła się niepowodzeniem, wszystkie próby wysłania wiadomości zakończyły się poniższymi błędami: ${newList} <button type='button' class='btn-close text-dark' aria-label='Close' onclick='CloseAlert()'></button>`);

                    break;
                case "Multiple-Partial":
                    console.log(`Operation partially successful - errors detected!`);

                    const uniquePartialErrors = response.errors.filter(findUniqueErrors);

                    var newPartialList = `<ul style="margin-top: 8px; margin-bottom: 0;">`;
                    uniquePartialErrors.forEach((error) => {
                        newPartialList += `<li>${error}</li>`;
                    })
                    newPartialList += "</ul>"

                    $("#status-message-sms").addClass("failed-status");
                    $("#status-message-sms").html(`Operacja zakończyła się częściowym powodzeniem (zakolejkowane: ${response.queued}, niewysłane: ${response.unsent}), Niektóre próby wysłania sms-a zakończyły się poniższymi błędami: ${newPartialList} <button type='button' class='btn-close text-dark' aria-label='Close' onclick='CloseAlert()'></button>`);
                    break;
            }       
            $("#status-message-sms").show();
        },
        error: function (error) {
            $("#status-message-sms").addClass("failed-status");
            $("#status-message-sms").html(`Wystąpił wewnętrzny błąd servera! Skontaktuj się z obsługą. <button type='button' class='btn-close text-dark' aria-label='Close' onclick='CloseAlert()'></button>`);
            $("#status-message-sms").show();
            console.error(error.responseText);
        }
    });
}

function checkIfAuthorizationSuccessful(password) {
    $.ajax({
        url: '/Home/CheckIfAuthorizationSuccessful',
        type: 'POST',
        data: JSON.stringify(password),
        contentType: 'application/json; charset=utf-8',
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
            console.error(error.responseText);
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
            $("#status-message-settings").addClass("success-status");
            $("#status-message-settings").html("Pomyślnie zapisano zmiany ustawień API. <button type='button' class='btn-close text-dark' aria-label='Close' onclick='CloseSettingsAlert()'></button>");
            $("#status-message-settings").show();
        },
        error: function (errorData) {
            console.error(errorData.responseText);
            $("#status-message-settings").addClass("failed-status");
            $("#status-message-settings").html("Wystąpił błąd podczas zapisywania zmian. Zmiany nie zostały zapisane. <button type='button' class='btn-close text-dark' aria-label='Close' onclick='CloseSettingsAlert()'></button>");
            $("#status-message-settings").show();
        }
    });
}

function fetchApiSettingsByName(apiName) {
    $.ajax({
        url: '/Home/FetchApiSettingsByName',
        type: 'GET',
        data: { apiName: apiName },
        contentType: 'application/json',
        success: function (apiSettingsObject) {
            $("#api-sender-name").val(apiSettingsObject.senderName);
            $("#fast-channel-checkbox").prop("checked", apiSettingsObject.fastChannel);
            $("#test-mode-checkbox").prop("checked", apiSettingsObject.testMode);
        },
        error: function (errorData) {
            console.error(errorData.responseText)
        }
    })
}

function addUserToGroup(link, assignContactIdGroupId) {
    $.ajax({
        url: '/Home/AddUserToGroup',
        type: 'GET',
        data: { groupId: assignContactIdGroupId[2], employeeId: assignContactIdGroupId[1] },
        contentType: 'application/json',
        success: function () {
            var groupMembersCount = $("#group-members-count").html();
            $("#group-members-count").html(parseInt(groupMembersCount) + 1);
            link.removeClass("contact-assign").addClass("contact-unassign");
            link.attr("href", `#unassign-${assignContactIdGroupId[1]}-${assignContactIdGroupId[2]}`)
            link.children("img").attr("src", "/icons/unassign-user.png").attr("title", "Wypisz z grupy");

            var nearestRow = link.closest('tr');
            var assignedGroups = nearestRow.find('td.centered-cell').html() == "Nie przypisano" ? [] : nearestRow.find('td.centered-cell').html().split(", ");
            assignedGroups.push($("#group-name").html());
            nearestRow.find('td.centered-cell').html(assignedGroups.join(", "));
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function assignGroupToUser(link, assignContactIdGroupId) {
    $.ajax({
        url: '/Home/AddUserToGroup',
        type: 'GET',
        data: { groupId: assignContactIdGroupId[2], employeeId: assignContactIdGroupId[1] },
        contentType: 'application/json',
        success: function () {
            link.removeClass("contact-group-assign").addClass("contact-group-unassign");
            link.attr("href", `#unassign-${assignContactIdGroupId[1]}-${assignContactIdGroupId[2]}`)
            link.children("img").attr("src", "/icons/unassign-user.png").attr("title", "Wypisz z grupy");
            var nearestRow = link.closest('tr');

            var groupMembersElement = nearestRow.find('.group-members');
            var currentHtml = groupMembersElement.html();
            var currentValue = parseInt(currentHtml);
            groupMembersElement.html(currentValue + 1);


            var assignedGroups = $("#groups-assigned-to-user").html() == "Nie przypisano" ? [] : $("#groups-assigned-to-user").html().split(", ");
            assignedGroups.push(nearestRow.find(".small-cell").html());
            $("#groups-assigned-to-user").html(assignedGroups.join(", "));
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function removeUserFromGroup(link, assignContactIdGroupId) {
    $.ajax({
        url: '/Home/RemoveUserFromGroup',
        type: 'GET',
        data: { groupId: assignContactIdGroupId[2], employeeId: assignContactIdGroupId[1] },
        contentType: 'application/json',
        success: function () {
            var groupMembersCount = $("#group-members-count").html();
            $("#group-members-count").html(parseInt(groupMembersCount) - 1);
            link.removeClass("contact-unassign").addClass("contact-assign");
            link.attr("href", `#assign-${assignContactIdGroupId[1]}-${assignContactIdGroupId[2]}`)
            link.children("img").attr("src", "/icons/assign-user.png").attr("title", "Dopisz do grupy");

            var nearestRow = link.closest('tr');
            var assignedGroups = nearestRow.find('td.centered-cell').html().split(", ");
            console.log($("#group-name").html());
            var index = assignedGroups.indexOf($("#group-name").html());
            assignedGroups.splice(index, 1);
            nearestRow.find('td.centered-cell').html(assignedGroups.join(", ") == "" ? "Nie przypisano" : assignedGroups.join(", "));
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function unassignGroupFromUser(link, assignContactIdGroupId) {
    $.ajax({
        url: '/Home/RemoveUserFromGroup',
        type: 'GET',
        data: { groupId: assignContactIdGroupId[2], employeeId: assignContactIdGroupId[1] },
        contentType: 'application/json',
        success: function () {
            link.removeClass("contact-group-unassign").addClass("contact-group-assign");
            link.attr("href", `#assign-${assignContactIdGroupId[1]}-${assignContactIdGroupId[2]}`)
            link.children("img").attr("src", "/icons/assign-user.png").attr("title", "Dopisz do grupy");

            var nearestRow = link.closest('tr');

            var groupMembersElement = nearestRow.find('.group-members');
            var currentHtml = groupMembersElement.html();
            var currentValue = parseInt(currentHtml);
            groupMembersElement.html(currentValue - 1);

            var assignedGroups = $("#groups-assigned-to-user").html().split(", ");
            var index = assignedGroups.indexOf(nearestRow.find(".group-name").html());
            assignedGroups.splice(index, 1);
            $("#groups-assigned-to-user").html(assignedGroups.join(", ") || "Nie przypisano");
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function importContacts() {

    function IterateOverListOfObjectsAndAppendToTable(object, tableId) {
        $.each(object.ListOfEmployees, function (index, employee) {
            var group = employee.employeeGroupNames;
            var email = employee.email || "Brak danych";

            group = (group == null || group.length === 0) ? "Nie przypisano" : group.join(", ");

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
            alert("Wystąpił błąd poczas eksportu danych do dokumentu Excel")
            console.error(error.responseText);
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
                        <span class="details-span" id="logTemplate-TemplateDescriptionDetail">${templateEntity.templateDescription || "Brak opisu"}</span>
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

    var groupDescription = groupAssignObject.group.groupDescription || "Brak opisu";

    var email = groupAssignObject.contact.email || "Brak danych";
    var hqAddress = groupAssignObject.contact.hqAddress || "Brak danych";
    var postalNumber = groupAssignObject.contact.postalNumber || "Brak danych";
    var city = groupAssignObject.contact.city || "Brak danych";
    var department = groupAssignObject.contact.department || "Brak danych";

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
                        <span class="details-span" id="logGroupAssign-GroupDescriptionDetail">${groupDescription}</span>
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
                            <span class="details-span" id="logGroupAssign-EmployeeEmailDetail">${email}</span>
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
                        <span class="details-span" id="logGroupAssign-EmployeeHQAddressDetail">${hqAddress}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeePostalNumberDetail">Kod pocztowy:</label>
                        <span class="details-span" id="logGroupAssign-EmployeePostalNumberDetail">${postalNumber}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeeCityDetail">Miasto:</label>
                        <span class="details-span" id="logGroupAssign-EmployeeCityDetail">${city}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logGroupAssign-EmployeeDepartmentDetail">Jednostka organizacyjna:</label>
                        <span class="details-span" id="logGroupAssign-EmployeeDepartmentDetail">${department}</span>
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

    var groupDescription = groupEntity.groupDescription || "Brak opisu";

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
                        <span class="details-span" id="logGroupAssign-GroupDescriptionDetail">${groupDescription}</span>
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

    if (smsGroupObject.apiUsed == "ServerSms") {
        var smsMessage = smsGroupObject.sms.settings.text || "Nie wpisano treści";
        var responseError = smsGroupObject.sms.serverResponse.error;
        var senderName = smsGroupObject.sms.settings.sender || "Nie określono";
        var fastChannel = smsGroupObject.sms.settings.speed;
    }
    else {
        var smsMessage = smsGroupObject.sms.settings.message || "Nie wpisano treści";
        var responseError = smsGroupObject.sms.serverResponse;
        var senderName = smsGroupObject.sms.settings.from || "Nie określono";
        var fastChannel = smsGroupObject.sms.settings.fast;
    }

    var chosenGroupId = smsGroupObject.sms.chosenGroupId || "Nie wybrano grupy";
    var additionalPhoneNumbers = smsGroupObject.sms.additionalPhoneNumbers;
    var additionalInformation = smsGroupObject.sms.additionalInformation || "Brak";
    var responseSuccess = smsGroupObject.sms.serverResponse;
    var groupDescription = smsGroupObject.group.groupDescription || "Brak opisu";

    var relatedObjects =
        `<span class="log-related-object-title">Wiadomość SMS</span>
                <hr style="margin-top: 20px";/>
                <span class="form-subtitle">Informacje</span>
                    <div class="row mb-10" style="margin-top: 20px;">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-SenderDetail">Nazwa nadawcy:</label>
                            <span class="details-span" id="logSMSGroup-SenderDetail">${senderName}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-ChosenGroupIdDetail">Id wybranej grupy:</label>
                            <span class="details-span" id="logSMSGroup-ChosenGroupIdDetail">${chosenGroupId}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-MessageDetail">Treść wiadomości:</label>
                            <span class="details-span" id = "logSMSGroup-MessageDetail" > ${smsMessage}</span>
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
                            <span class="details-span" id="logSMSGroup-AdditionalInformationDetail">${additionalInformation}</span>
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
                            <span class="details-span" id="logSMSGroup-SpeedDetail">${fastChannel == "1" ? "Tak" : "Nie"}</span>
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
                                <span class="details-span" id="logSMSGroup-CodeDetail">${responseError.code}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-ErrorTypeDetail">Typ błędu:</label>
                                <span class="details-span" id="logSMSGroup-ErrorTypeDetail">${responseError.type}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-ErrorMessageDetail">Wiadomość błędu:</label>
                            <span class="details-span" id="logSMSGroup-ErrorMessageDetail">${responseError.message}</span>
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
                                <span class="details-span" id="logSMSGroup-CodeDetail">${responseError.errorCode}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logSMSGroup-ErrorMessageDetail">Wiadomość błędu:</label>
                                <span class="details-span" id="logSMSGroup-ErrorMessageDetail">${responseError.errorMessage}</span>
                        </div>
                    </div>
                `;
        }

    }
    else {
        if (smsGroupObject.apiUsed == "ServerSms") {
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
                        <span class="details-span" id="logSMSGroup-GroupDescriptionDetail">${groupDescription}</span>
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

    if (smsNoGroupObject.apiUsed == "ServerSms") {
        var smsMessage = smsNoGroupObject.sms.settings.text || "Nie wpisano treści";
        var responseError = smsNoGroupObject.sms.serverResponse.error;
        var senderName = smsNoGroupObject.sms.settings.sender || "Nie określono";
        var fastChannel = smsNoGroupObject.sms.settings.speed;
    }
    else {
        var smsMessage = smsNoGroupObject.sms.settings.message || "Nie wpisano treści";;
        var responseError = smsNoGroupObject.sms.serverResponse;
        var senderName = smsNoGroupObject.sms.settings.from || "Nie określono";;
        var fastChannel = smsNoGroupObject.sms.settings.fast;
    }

    var chosenGroupId = smsNoGroupObject.sms.chosenGroupId || "Nie wybrano grupy";
    var additionalPhoneNumbers = smsNoGroupObject.sms.additionalPhoneNumbers;
    var additionalInformation = smsNoGroupObject.sms.additionalInformation || "Brak";
    var responseSuccess = smsNoGroupObject.sms.serverResponse;

    var relatedObjects =
        `
            <span class="log-related-object-title">Wiadomość SMS</span>
            <hr style="margin-top: 20px";/>
            <span class="form-subtitle">Informacje</span>
            <div class="row mb-10" style="margin-top: 20px;">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logSMSNoGroup-SenderDetail">Nazwa nadawcy:</label>
                    <span class="details-span" id="logSMSNoGroup-SenderDetail">${senderName}</span>
                </div>
            </div>
            <div class="row mb-10">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logSMSNoGroup-ChosenGroupIdDetail">Id wybranej grupy:</label>
                    <span class="details-span" id="logSMSNoGroup-ChosenGroupIdDetail">${chosenGroupId}</span>
                </div>
            </div>
            <div class="row mb-10">
                <div class="col d-flex">
                    <label class="logs-detail-label" for="logSMSNoGroup-MessageDetail">Treść wiadomości:</label>
                    <span class="details-span" id = "logSMSGroup-MessageDetail" > ${smsMessage}</span>

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
                    <span class="details-span" id="logSMSNoGroup-AdditionalInformationDetail">${additionalInformation}</span>
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
                    <span class="details-span" id="logSMSNoGroup-SpeedDetail">${fastChannel == "1" ? "Tak" : "Nie"}</span>
                </div>
            </div>
            <hr style="margin-top: 20px";/>
            <span class="form-subtitle">Odpowiedź servera</span>
            `;

    if (type == "Błąd") {
        if (smsNoGroupObject.apiUsed == "ServerSms") {
            relatedObjects +=
                `
                <div class="row mb-10" style="margin-top: 20px;">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSNoGroup-CodeDetail">Kod błędu:</label>
                        <span class="details-span" id="logSMSNoGroup-CodeDetail">${responseError.code}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSNoGroup-ErrorTypeDetail">Typ błędu:</label>
                        <span class="details-span" id="logSMSNoGroup-ErrorTypeDetail">${responseError.type}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSNoGroup-ErrorMessageDetail">Wiadomość błędu:</label>
                        <span class="details-span" id="logSMSNoGroup-ErrorMessageDetail">${responseError.message}</span>
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
                        <span class="details-span" id="logSMSNoGroup-CodeDetail">${responseError.errorCode}</span>
                    </div>
                </div>
                <div class="row mb-10">
                    <div class="col d-flex">
                        <label class="logs-detail-label" for="logSMSNoGroup-ErrorMessageDetail">Wiadomość błędu:</label>
                        <span class="details-span" id="logSMSNoGroup-ErrorMessageDetail">${responseError.errorMessage}</span>
                    </div>
                </div>
                `;
        }

    }
    else {
        if (smsNoGroupObject.apiUsed == "ServerSms") {
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

    var email = contactEntity.email || "Brak danych";
    var hqAddress = contactEntity.hqAddress || "Brak danych";
    var postalNumber = contactEntity.postalNumber || "Brak danych";
    var city = contactEntity.city || "Brak danych";
    var department = contactEntity.department || "Brak danych";


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
                            <span class="details-span" id="logDontact-EmployeeEmailDetail">${email}</span>
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
                            <span class="details-span" id="logContact-EmployeeHQAddressDetail">${hqAddress}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeePostalNumberDetail">Kod pocztowy:</label>
                            <span class="details-span" id="logContact-EmployeePostalNumberDetail">${postalNumber}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeeCityDetail">Miasto:</label>
                            <span class="details-span" id="logContact-EmployeeCityDetail">${city}</span>
                        </div>
                    </div>
                    <div class="row mb-10">
                        <div class="col d-flex">
                            <label class="logs-detail-label" for="logContact-EmployeeDepartmentDetail">Jednostka organizacyjna:</label>
                            <span class="details-span" id="logContact-EmployeeDepartmentDetail">${department}</span>
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
            var lastId = $("#template-table").attr('last-id');
            var firstId = $("#template-table").attr('first-id');
            PaginateTemplatesAndPopulateTable(firstId, lastId, 11, null);
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function CloseAlert() {
    $("#status-message-sms").hide();
}

function CloseSettingsAlert() {
    $("#status-message-settings").hide();
}

function DeleteContact(contactId) {

    $.ajax({
        url: '/Home/DeleteContact',
        type: 'GET',
        contentType: 'application/json',
        data: { id: contactId },
        success: function () {
            var lastId = $("#contacts-table").attr('last-id');
            var firstId = $("#contacts-table").attr('first-id');
            PaginateContactsAndPopulateTable(firstId, lastId, 11, null);

        },
        error: function (error) {
            console.error(error.responseText);
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
            var lastId = $("#group-table").attr('last-id');
            var firstId = $("#group-table").attr('first-id');
            PaginateGroupsAndPopulateTable(firstId, lastId, 11, null);
        },
        error: function (error) {
            console.error(error.responseText);
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
        success: function () {
            var affectedRow = $("#template-table tr").find(`a[href="#details-${templateId}"]`).closest("tr");
            affectedRow.find(".template-name").html(templateName);
            affectedRow.find(".template-description").html(templateDescription || "Brak opisu");
            affectedRow.find(".template-content").html(templateContent);
        },
        error: function (error) {
            console.error(error.responseText);
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

    $.ajax({
        url: '/Home/EditContact',
        type: 'GET',
        contentType: 'application/json',
        data: { contactId: contactId, contactName: name, contactSurname: surname, email: email, phone: phoneNumber, address: address, zip: zip, city: city, department: department, isActive: isActive },
        success: function () {
            var affectedRow = $("#contacts-table tr").find(`a[href="#details-${contactId}"]`).closest("tr");
            affectedRow.find(".contact-name").html(name);
            affectedRow.find(".contact-surname").html(surname);
            affectedRow.find(".contact-email").html(email || "Brak danych");
            affectedRow.find(".contact-phone").html(phoneNumber);
            affectedRow.find("span").html(isActive == "yes" ? "Aktywny" : "Nieaktywny");
            if (isActive == "yes") {
                affectedRow.find("span").removeClass("active-pill").removeClass("inactive-pill").addClass("active-pill");
            }
            else {
                affectedRow.find("span").removeClass("active-pill").removeClass("inactive-pill").addClass("inactive-pill");
            }
        },
        error: function (error) {
            console.error(error.responseText);
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
        success: function () {
            var affectedRow = $("#group-table tr").find(`a[href="#details-${groupId}"]`).closest("tr");
            affectedRow.find(".group-name").html(groupName);
            affectedRow.find(".group-description").html(groupDescription || "Brak opisu");
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function FetchAllTemplatesAndPopulateTable() {
    $.ajax({
        url: '/Home/FetchAllTemplates',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfTemplates) {
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
            console.error(error.responseText);
        }
    })
}

function PaginateTemplatesAndPopulateTable(firstId, lastId, pageSize, moveForward) {
    $.ajax({
        url: '/Home/PaginateTemplates',
        type: 'GET',
        data: {firstId, lastId, pageSize, moveForward },
        contentType: 'application/json',
        success: function (response) {
            const { paginatedTemplates, hasMorePages } = response;
            const templateListBody = $('.template-list tbody');
            const templateNextButtonContainer = $("#template-next-button-container");
            const templatePreviousButtonContainer = $("#template-previous-button-container");
            const templatePageCounter = $("#template-page-counter");

            templatePageCounter.hide();
            templateListBody.empty();
            templateNextButtonContainer.empty();
            templatePreviousButtonContainer.empty();

            paginatedTemplates.forEach(template => {
                const description = template.templateDescription || "Brak opisu";

                var newRow = `
                    <tr>
                        <td class="small-cell template-name">${template.templateName}</td>
                        <td class="medium-cell template-description">${description}</td>
                        <td class="big-cell template-content">${template.templateContent}</td>
                        <td class="centered-cell">
                            <a href="#details-${template.templateId}" class="icon-list template-details"><img src="/icons/view-doc.png" title="Szczegóły"/></a>
                            <a href="#edit-${template.templateId}" class="icon-list template-edit"><img src="/icons/edit.png" title="Edytuj"/></a>
                            <a href="#delete-${template.templateId}" class="icon-list template-delete"><img src="/icons/trash.png" title="Usuń"/></a>
                        </td>
                    </tr>
                `;

                templateListBody.append(newRow);
            })

            if (hasMorePages) {
                templateNextButtonContainer.append(`
                    <button class="arrow-button" id="templates-list-next-page-button" type="button">
                        <span class="list-arrow-forward">
                            <img src="/icons/arrow-next.png" />
                        </span>
                    </button>`);
                templatePageCounter.show();
            }

            if (moveForward != null) {
                const pageCounterValue = parseInt(templatePageCounter.html());
                templatePageCounter.html(`${moveForward ? pageCounterValue + 1 : pageCounterValue - 1}`);
            }
            

            if (parseInt(templatePageCounter.html()) > 1) {
                templatePreviousButtonContainer.append(`
                    <button class="arrow-button" id="templates-list-previous-page-button" type="button">
                        <span class="list-arrow-back">
                            <img src="/icons/arrow-previous.png"/>
                        </span>
                    </button>
                `);
                templatePageCounter.show();
            }

            $('.template-list').attr("last-id", `${paginatedTemplates[paginatedTemplates.length - 1].templateId}`);
            $('.template-list').attr("first-id", `${paginatedTemplates[0].templateId}`)
        },
        error: function (errorData) {
            console.error(errorData.responseText);
        }
    })
}

function FetchAllContactsAndPopulateTable() {
    $.ajax({
        url: '/Home/FetchAllContacts',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfContacts) {
            $('.contacts-list tbody').empty();
            $.each(listOfContacts, function (index, item) {

                var groupNames = item.employeeGroupNames;
                var email = item.email || "Brak danych";

                groupNames = (groupNames == null || groupNames.length == 0) ? "Nie przypisano" : groupNames.join(", ")

                var isActiveRow = item.isActive ? '<td class="centered-cell contact-activity"><span class="active-pill">Aktywny<span></td>' : '<td class="centered-cell contact-activity"><span class="inactive-pill">Nieaktywny<span></td>'

                var newRow = `<tr>
                            <td class="tiny-cell contact-name">${item.name}</td>
                            <td class="tiny-cell contact-surname">${item.surname}</td>
                            <td class="small-cell contact-email">${email}</td>
                            <td class="small-cell contact-phone">${item.phoneNumber}</td>
                            ${isActiveRow}
                            <td class="centered-cell contact-groups">${groupNames}</td>
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
            console.error(error.responseText);
        }
    });
}

function PaginateContactsAndPopulateTable(firstId, lastId, pageSize, moveForward) {
    $.ajax({
        url: 'Home/PaginateContacts',
        type: 'GET',
        data: {firstId, lastId,  pageSize, moveForward },
        contentType: 'application/json',
        success: function (response)
        {
            const { paginatedContacts, hasMorePages } = response;
            const contactListBody = $('.contacts-list tbody');
            const contactNextButtonContainer = $("#contact-next-button-container");
            const contactPreviousButtonContainer = $("#contact-previous-button-container");
            const contactPageCounter = $("#contact-page-counter");

            contactPageCounter.hide();
            contactListBody.empty();
            contactPreviousButtonContainer.empty();
            contactNextButtonContainer.empty();

            paginatedContacts.forEach(contact => {

                var groupNames = contact.employeeGroupNames;
                var email = contact.email || "Brak danych";

                groupNames = (groupNames == null || groupNames.length == 0) ? "Nie przypisano" : groupNames.join(", ")
                var isActiveRow = contact.isActive ? '<td class="centered-cell contact-activity"><span class="active-pill">Aktywny<span></td>' : '<td class="centered-cell contact-activity"><span class="inactive-pill">Nieaktywny<span></td>'

                var newRow = `
                    <tr>
                        <td class="tiny-cell contact-name">${contact.name}</td>
                        <td class="tiny-cell contact-surname">${contact.surname}</td>
                        <td class="small-cell contact-email">${email}</td>
                        <td class="small-cell contact-phone">${contact.phoneNumber}</td>
                        ${isActiveRow}
                        <td class="centered-cell contact-groups">${groupNames}</td>
                        <td class="centered-cell" style="min-width: 163px;">
                            <a href="#assign-${contact.employeeId}" class="icon-list contact-assign-groups"><img src="/icons/assign-users.png" title="Przypisz grupy"/></a>
                            <a href="#details-${contact.employeeId}" class="icon-list contact-details"><img src="/icons/view-doc.png" title="Szczegóły"/></a>
                            <a href="#edit-${contact.employeeId}" class="icon-list contact-edit"><img src="/icons/edit.png" title="Edytuj"/></a>
                            <a href="#delete-${contact.employeeId}" class="icon-list contact-delete"><img src="/icons/trash.png" title="Usuń"/></a>
                        </td>
                    </tr>
                `;

                contactListBody.append(newRow);
            });

            if (hasMorePages) {
                contactNextButtonContainer.append(`
                    <button class="arrow-button" id="contacts-list-next-page-button" type="button">
                        <span class="list-arrow-forward">
                            <img src="/icons/arrow-next.png" />
                        </span>
                    </button>`
                );
                contactPageCounter.show();
            }

            if (moveForward != null) {
                const pageCounterValue = parseInt(contactPageCounter.html());
                contactPageCounter.html(`${moveForward ? pageCounterValue + 1 : pageCounterValue - 1}`)
            }

            if (parseInt(contactPageCounter.html()) > 1) {
                contactPreviousButtonContainer.append(`
                    <button class="arrow-button" id="contacts-list-previous-page-button" type="button">
                        <span class="list-arrow-back">
                            <img src="/icons/arrow-previous.png"/>
                        </span>
                    </button>
                `);
                contactPageCounter.show();
            }

            $('.contacts-list').attr("last-id", `${paginatedContacts[paginatedContacts.length - 1].employeeId}`);
            $('.contacts-list').attr("first-id", `${paginatedContacts[0].employeeId}`);
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}



function PaginateAssignContactsAndPopulateTable(firstId, lastId, pageSize, moveForward) {
    $.ajax({
        url: 'Home/PaginateContacts',
        type: 'GET',
        data: { firstId, lastId, pageSize, moveForward },
        contentType: 'application/json',
        success: function (response) {
            const { paginatedContacts, hasMorePages } = response;
            const contactListBody = $('#group-assign-contact-list-table tbody');
            const contactNextButtonContainer = $("#contact-assign-next-button-container");
            const contactPreviousButtonContainer = $("#contact-assign-previous-button-container");
            const contactPageCounter = $("#contact-assign-page-counter");

            contactPageCounter.hide();
            contactListBody.empty();
            contactPreviousButtonContainer.empty();
            contactNextButtonContainer.empty();

            paginatedContacts.forEach(contact => {

                var groupNames = contact.employeeGroupNames;

                groupNames = (groupNames == null || groupNames.length == 0) ? "Nie przypisano" : groupNames.join(", ")
                var isActiveRow = contact.isActive ? '<td class="tiny-centered-cell contact-activity"><span class="active-pill">Aktywny<span></td>' : '<td class="tiny-centered-cell contact-activity"><span class="inactive-pill">Nieaktywny<span></td>'

                var newRow = `
                    <tr>
                        <td class="tiny-cell contact-name">${contact.name}</td>
                        <td class="tiny-cell contact-surname">${contact.surname}</td>
                        <td class="tiny-cell contact-phone">${contact.phoneNumber}</td>
                        ${isActiveRow}
                        <td class="centered-cell contact-groups">${groupNames}</td>
                        <td class="tiny-centered-cell">
                `;

                if (groupAssignMembersIds.includes(contact.employeeId)) {

                    newRow += `<a href="#unassign-${contact.employeeId}-${grouAssignGroupId}" class="icon-list contact-unassign"><img src="/icons/unassign-user.png" title="Wypisz z grupy"/></a>
                                        </td>
                                        </tr>`;
                }
                else {
                    newRow += `<a href="#assign-${contact.employeeId}-${grouAssignGroupId}" class="icon-list contact-assign"><img src="/icons/assign-user.png" title="Dopisz do grupy" /></a>
                                        </td>
                                        </tr>`;
                }

                contactListBody.append(newRow);
            });

            if (hasMorePages) {
                contactNextButtonContainer.append(`
                    <button class="arrow-button" id="contacts-assign-list-next-page-button" type="button">
                        <span class="list-arrow-forward">
                            <img src="/icons/arrow-next.png" />
                        </span>
                    </button>`
                );
                contactPageCounter.show();
            }

            if (moveForward != null) {
                const pageCounterValue = parseInt(contactPageCounter.html());
                contactPageCounter.html(`${moveForward ? pageCounterValue + 1 : pageCounterValue - 1}`)
            }

            if (parseInt(contactPageCounter.html()) > 1) {
                contactPreviousButtonContainer.append(`
                    <button class="arrow-button" id="contacts-assign-list-previous-page-button" type="button">
                        <span class="list-arrow-back">
                            <img src="/icons/arrow-previous.png"/>
                        </span>
                    </button>
                `);
                contactPageCounter.show();
            }

            $('#group-assign-contact-list-table').attr("last-id", `${paginatedContacts[paginatedContacts.length - 1].employeeId}`);
            $('#group-assign-contact-list-table').attr("first-id", `${paginatedContacts[0].employeeId}`);
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}



function FetchAllGroupsAndPopulateTable() {
    $.ajax({
        url: '/Home/FetchAllGroups',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfGroups) {
            $('.group-list tbody').empty();

            $.each(listOfGroups, function (index, item) {
                var groupDescription = item.groupDescription || "Brak opisu";

                var newRow = `
                    <tr>
                        <td class="small-cell group-name">${item.groupName}</td>
                        <td class="big-cell group-description">${groupDescription}</td>
                        <td class="tiny-centered-cell">${item.membersIds.length}</td>
                        <td class="centered-cell" id="group-options-container" style="min-width: 205px;">
                            <a href="#assign-${item.groupId}" class="icon-list group-assign"><img src="/icons/assign-users.png" title="Przypisz użytkowników"/></a>
                            <a href="#details-${item.groupId}" class="icon-list group-details"><img src="/icons/view-doc.png" title="Szczegóły"/></a>
                            <a href="#edit-${item.groupId}" class="icon-list group-edit"><img src="/icons/edit.png" title="Edytuj"/></a>
                            <a href="#delete-${item.groupId}" class="icon-list group-delete"><img src="/icons/trash.png" title="Usuń"/></a>
                        </td>
                    </tr>`
                ;

                $('.group-list tbody').append(newRow);
            });
        },
        error: function (error) {
            console.error(error.responseText);
        }
    })
}

function PaginateGroupsAndPopulateTable(firstId, lastId, pageSize, moveForward) {
    $.ajax({
        url: '/Home/PaginateGroups',
        type: 'GET',
        data: {firstId, lastId, pageSize, moveForward },
        contentType: 'application/json',
        success: function (response) {
            const { paginatedGroups, hasMorePages } = response;
            const groupListBody = $('.group-list tbody');
            const groupNextButtonContainer = $("#group-next-button-container");
            const groupPreviousButtonContainer = $("#group-previous-button-container");
            const groupPageCounter = $("#group-page-counter");

            groupPageCounter.hide();
            groupListBody.empty();
            groupNextButtonContainer.empty();
            groupPreviousButtonContainer.empty();

            paginatedGroups.forEach(group => {
                const description = group.groupDescription || "Brak opisu"

                var newRow = `
                    <tr>
                        <td class="small-cell group-name">${group.groupName}</td>
                        <td class="big-cell group-description">${description}</td>
                        <td class="tiny-centered-cell">${group.membersIds.length}</td>
                        <td class="centered-cell" id="group-options-container" style="min-width: 205px !important;">
                            <a href="#assign-${group.groupId}" class="icon-list group-assign"><img src="/icons/assign-users.png" title="Przypisz użytkowników"/></a>
                            <a href="#details-${group.groupId}" class="icon-list group-details"><img src="/icons/view-doc.png" title="Szczegóły"/></a>
                            <a href="#edit-${group.groupId}" class="icon-list group-edit"><img src="/icons/edit.png" title="Edytuj"/></a>
                            <a href="#delete-${group.groupId}" class="icon-list group-delete"><img src="/icons/trash.png" title="Usuń"/></a>
                        </td>
                    </tr>
                `;

                groupListBody.append(newRow);
            });

            if (hasMorePages) {
                groupNextButtonContainer.append(`
                    <button class="arrow-button" id="groups-list-next-page-button" type="button">
                        <span class="list-arrow-forward">
                            <img src="/icons/arrow-next.png" />
                        </span>
                    </button>`
                );
                groupPageCounter.show();
            }

            if (moveForward != null) {
                const pageCounterValue = parseInt(groupPageCounter.html());
                groupPageCounter.html(`${moveForward ? pageCounterValue + 1 : pageCounterValue - 1}`);
            }

            if (parseInt($("#group-page-counter").html()) > 1) {
                groupPreviousButtonContainer.append(`
                    <button class="arrow-button" id="groups-list-previous-page-button" type="button">
                        <span class="list-arrow-back">
                            <img src="/icons/arrow-previous.png"/>
                        </span>
                    </button>
                `);
                groupPageCounter.show();
            }

            $('.group-list').attr("last-id", `${paginatedGroups[paginatedGroups.length - 1].groupId}`);
            $('.group-list').attr("first-id", `${paginatedGroups[0].groupId}`);
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function PaginateAssignGroupsAndPopulateTable(firstId, lastId, pageSize, moveForward) {
    $.ajax({
        url: '/Home/PaginateGroups',
        type: 'GET',
        data: { firstId, lastId, pageSize, moveForward },
        contentType: 'application/json',
        success: function (response) {
            const { paginatedGroups, hasMorePages } = response;
            const groupListBody = $('#group-assign-group-list-table tbody');
            const groupNextButtonContainer = $("#group-assign-next-button-container");
            const groupPreviousButtonContainer = $("#group-assign-previous-button-container");
            const groupPageCounter = $("#group-assign-page-counter");

            groupPageCounter.hide();
            groupListBody.empty();
            groupNextButtonContainer.empty();
            groupPreviousButtonContainer.empty();

            paginatedGroups.forEach(group => {
                const description = group.groupDescription || "Brak opisu"

                var newRow = `
                    <tr>
                        <td class="small-cell group-name">${group.groupName}</td>
                        <td class="big-cell group-description">${description}</td>
                        <td class="tiny-centered-cell group-members">${group.membersIds.length}</td>
                        <td class="tiny-centered-cell">
                `;

                if (group.membersIds.includes(parseInt(groupAssignContactId))) {

                    newRow += `<a href="#unassign-${groupAssignContactId}-${group.groupId}" class="icon-list contact-group-unassign"><img src="/icons/unassign-user.png" title="Wypisz z grupy"/></a>
                                        </td>
                                        </tr>`;
                }
                else {
                    newRow += `<a href="#assign-${groupAssignContactId}-${group.groupId}" class="icon-list contact-group-assign"><img src="/icons/assign-user.png" title="Dopisz do grupy"/></a>
                                        </td>
                                        </tr>`;
                }

                groupListBody.append(newRow);
            });

            if (hasMorePages) {
                groupNextButtonContainer.append(`
                    <button class="arrow-button" id="groups-assign-list-next-page-button" type="button">
                        <span class="list-arrow-forward">
                            <img src="/icons/arrow-next.png" />
                        </span>
                    </button>`
                );
                groupPageCounter.show();
            }

            if (moveForward != null) {
                const pageCounterValue = parseInt(groupPageCounter.html());
                groupPageCounter.html(`${moveForward ? pageCounterValue + 1 : pageCounterValue - 1}`);
            }

            if (parseInt(groupPageCounter.html()) > 1) {
                groupPreviousButtonContainer.append(`
                    <button class="arrow-button" id="groups-assign-list-previous-page-button" type="button">
                        <span class="list-arrow-back">
                            <img src="/icons/arrow-previous.png"/>
                        </span>
                    </button>
                `);
                groupPageCounter.show();
            }

            $('#group-assign-group-list-table').attr("last-id", `${paginatedGroups[paginatedGroups.length - 1].groupId}`);
            $('#group-assign-group-list-table').attr("first-id", `${paginatedGroups[0].groupId}`);
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function FetchAllLogsAndPopulateTable() {
    $.ajax({
        url: '/Home/FetchAllLogs',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfLogs) {
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
            console.error(error.responseText);
        }
    })
}

function PaginateLogsAndPopulateTable(firstId, lastId, pageSize, moveForward) {
    $.ajax({
        url: '/Home/PaginateLogs',
        type: 'GET',
        data: {firstId, lastId, pageSize, moveForward },
        contentType: 'application/json',
        success: function (response) {
            const { paginatedLogs, hasMorePages } = response;
            const logListBody = $('#log-table tbody');
            const logNextButtonContainer = $("#log-next-button-container");
            const logPreviousButtonContainer = $("#log-previous-button-container");
            const logPageCounter = $("#log-page-counter");

            logPageCounter.hide();
            logListBody.empty();
            logNextButtonContainer.empty();
            logPreviousButtonContainer.empty();

            paginatedLogs.forEach(log => {
                var logTypeRow = log.logType == "Błąd" ? `<td class="tiny-centered-cell"><span class="error-pill">${log.logType}</span></td>` : `<td class="tiny-centered-cell"><span class="info-pill">${log.logType}</span></td>`;

                var newRow = `
                    <tr>
                        ${logTypeRow}
                        <td class="tiny-cell">${log.logSource}</td>
                        <td class="big-cell">${log.logMessage}</td>
                        <td class="centered-cell" style="min-width:105px;">${new Date(log.logCreationDate).toLocaleString('pl-PL')}</td>
                        <td class="tiny-centered-cell">
                            <a href="#details-${log.logId}" class="icon-list log-details"><img src="/icons/view-doc.png" title="Szczegóły"/></a>
                        </td>
                    </tr>`
                    ;

                logListBody.append(newRow);
            });

            if (hasMorePages) {
                logNextButtonContainer.append(`
                    <button class="arrow-button" id="logs-list-next-page-button" type="button">
                        <span class="list-arrow-forward">
                            <img src="/icons/arrow-next.png" />
                        </span>
                    </button>`
                );
                logPageCounter.show();
            }

            if (moveForward != null) {
                const pageCounterValue = parseInt(logPageCounter.html());
                logPageCounter.html(`${moveForward ? pageCounterValue + 1 : pageCounterValue - 1}`);
            }

            if (parseInt(logPageCounter.html()) > 1) {
                logPreviousButtonContainer.append(`
                    <button class="arrow-button" id="logs-list-previous-page-button" type="button">
                        <span class="list-arrow-back">
                            <img src="/icons/arrow-previous.png"/>
                        </span>
                    </button>`
                );

                logPageCounter.show();
            }

            $('.log-list').attr("last-id", `${paginatedLogs[paginatedLogs.length - 1].logId}`);
            $('.log-list').attr("first-id", `${paginatedLogs[0].logId}`);
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function PopulateTableForChooseGroupForSMS() {
    $.ajax({
        url: '/Home/FetchAllValidGroups',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfGroups) {
            $('#choose-group-for-sms-table tbody').empty();

            $.each(listOfGroups, function (index, item) {

                var description = item.groupDescription || "Brak opisu";

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
            console.error(error.responseText);
        }
    })
}

function PopulateTableForChooseTemplateForSMS() {
    $.ajax({
        url: '/Home/FetchAllTemplates',
        type: 'GET',
        contentType: 'application/json',
        success: function (listOfTemplates) {
            $('#choose-template-for-sms-table tbody').empty();

            $.each(listOfTemplates, function (index, item) {

                var templateDescription = item.templateDescription || "Brak opisu";

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
            console.error(error.responseText);
        }
    });
}

let groupAssignMembersIds;
let groupAssignContactId;
let groupAssignGroupId;

function PopulateTablesForAssigningUsersToGroups(groupId) {
    grouAssignGroupId = groupId;

    $.ajax({
        url: '/Home/GetGroupById',
        type: 'GET',
        data: { id: groupId },

        contentType: 'application/json',
        success: function (groupEntity) {

            groupAssignMembersIds = groupEntity.membersIds;

            var groupDescription = groupEntity.groupDescription || "Brak opisu";
            $("#group-assign-chosen-group-table tbody").empty();

            var newRow = `<tr>
                        <td class="small-cell" id="group-name">${groupEntity.groupName}</td>
                        <td class="big-cell">${groupDescription}</td>
                        <td class="tiny-centered-cell" id="group-members-count">${groupEntity.membersIds.length}</td>
                        </tr>`;

            $('#group-assign-chosen-group-table').append(newRow);

            PaginateAssignContactsAndPopulateTable(null, 0, 7, true);

            $(".groups-list-container").hide();
            $(".groups-options-container-assign").show();
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function PopulateTablesForAssigningGroupsToUsers(contactId) {
    groupAssignContactId = contactId;

    $.ajax({
        url: '/Home/GetContactById',
        type: 'GET',
        data: { id: contactId },
        contentType: 'application/json',

        success: function (contactEntity) {
            $("#group-assign-chosen-contact-table tbody").empty();
            var newRow = `<tr>
                        <td class="tiny-cell">${contactEntity.name}</td>
                        <td class="tiny-cell">${contactEntity.surname}</td>
                        <td class="small-cell">${contactEntity.phoneNumber}</td>
                        <td class="centered-cell">
                        <span class="${contactEntity.isActive ? " active-pill" : "inactive - pill"}" > ${contactEntity.isActive ? "Aktywny" : "Nieaktywny"}</span >
                        </td>
                        <td class="centered-cell" id="groups-assigned-to-user">${contactEntity.employeeGroupNames.join(", ")}</td>
                        </tr>
                        `;
            $("#group-assign-chosen-contact-table tbody").append(newRow);

            PaginateAssignGroupsAndPopulateTable(null, 0, 7, true)

            $(".contacts-list-container").hide();
            $(".contact-options-container-assign ").show();
        },
        error: function (error) {
            console.error(error.responseText);
        }
    })
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
        success: function (template) {
            var newRow = `
            <tr>
                <td class="small-cell template-name">${template.templateName}</td>
                <td class="medium-cell template-description">${template.templateDescription || "Brak opisu"}</td>
                <td class="big-cell template-content">${template.templateContent}</td>
                <td class=centered-cell>
                    <a href="#details-${template.templateId}" class="icon-list template-details">
                        <img src="/icons/view-doc.png" title="Szczegóły">
                    </a>
                    <a href="#edit-${template.templateId}" class="icon-list template-edit">
                        <img src="/icons/edit.png" title="Edytuj">
                    </a>
                    <a href="#delete-${template.templateId}" class="icon-list template-delete">
                        <img src="/icons/trash.png" title="Usuń">
                    </a>
                </td>
            </tr>
            `
            $("#template-table tbody").append(newRow);
        },
        error: function (error) {
            console.error(error.responseText);
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
        success: function (contact) {
            var newRow = `
            <tr>
                <td class="tiny-cell contact-name">${contact.name}</td>
                <td class="tiny-cell contact-surname">${contact.surname}</td>
                <td class="small-cell contact-email">${contact.email || "Brak opisu"}</td>
                <td class="small-cell contact-phone">${contact.phoneNumber}</td>
                <td class="centered-cell contact-activity">
                    <span class="${contact.isActive ? "active-pill" : "inactive-pill"}">${contact.isActive ? "Aktywny" : "Nieaktywny"}</span>
                </td>
                <td class="centered-cell contact-groups">Nie przypisano</td>
                <td class="centered-cell">
                    <a href="#details-${contact.employeeId}" class="icon-list contact-details">
                        <img src="/icons/view-doc.png" title="Szczegóły">
                    </a>
                    <a href="#edit-${contact.employeeId}" class="icon-list contact-edit">
                        <img src="/icons/edit.png" title="Edytuj">
                    </a>
                    <a href="#delete-${contact.employeeId}" class="icon-list contact-delete">
                        <img src="/icons/trash.png" title="Usuń">
                    </a>
                </td>
            </tr>
            `;
            $("#contacts-table tbody").append(newRow);
        },
        error: function (error) {
            console.error(error.responseText);
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
        success: function (group) {
            var newRow = `
            <tr>
                <td class="small-cell group-name">${group.groupName}</td>
                <td class="big-cell group-description">${group.groupDescription || "Brak danych"}</td>
                <td class="tiny-centered-cell">0</td>
                <td class="centered-cell" style="minwidth: 205px;">
                    <a href="#assign-${group.groupId}" class="icon-list group-assign">
                        <img src="/icons/assign-users.png" title="Przypisz użytkowników">
                    </a>
                    <a href="#details-${group.groupId}" class="icon-list group-details">
                        <img src="/icons/view-doc.png" title="Szczegóły">
                    </a>
                    <a href="#edit-${group.groupId}" class="icon-list group-edit">
                        <img src="/icons/edit.png" title="Edytuj">
                    </a>
                    <a href="#delete-${group.groupId}" class="icon-list group-delete">
                        <img src="/icons/trash.png" title="Usuń">
                    </a>
                </td>
            </tr>
            `;
            $("#group-table tbody").append(newRow);
        },
        error: function (error) {
            console.error(error.responseText);
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
    $(".templates-options-container .left span").html("Tworzenie szablonu")
    $(".template-options-form").attr('id', 'create-template-form');
    $("#createOrEditButtonTemplates").html("Utwórz szablon");
    $(".templates-list-container").hide();
    $(".templates-options-container").show();
}

function OpenCreateContactWindow() {
    document.getElementById("isActiveYes").checked = true;
    $(".contacts-options-container .left span").html("Tworzenie kontaktu")
    $(".contact-options-form").attr('id', 'create-contact-form');
    $("#createOrEditButtonContacts").html("Utwórz kontakt");
    $(".contacts-list-container").hide();
    $(".contacts-options-container").show();
}

function OpenCreateGroupWindow() {
    const messageEle = document.getElementById('groupDescriptionInput');
    document.getElementById('symbolCounterAddGroups').innerHTML = messageEle.value.length + "/" + messageEle.getAttribute('maxlength');
    $(".groups-options-container .left span").html("Tworzenie grupy");
    $(".group-options-form").attr('id', 'create-group-form');
    $("#createOrEditButtonGroups").html("Utwórz grupę");
    $(".groups-list-container").hide();
    $(".groups-options-container").show();
}

function openCreateUserWindow() {
    $(".users-options-container .left span").html("Tworzenie użytkownika");
    $("#create-edit-user-form").attr("operation-type", 'create');
    $("#createOrEditUserButton").html("Utwórz użytkownika");
    getRolesForUserCreation();
    $(".users-list-container").hide();
    $(".users-options-container").show();
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
    var firstId = $("contacts-table").attr("first-id");
    PaginateContactsAndPopulateTable(firstId, null, 11, null);
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

function GoBackToUserList() {
    document.querySelector("#create-edit-user-form").reset();
    $("#create-edit-user-formm").removeAttr("operation-type");
    $("#user-role").empty();
    $("#status-create-edit-user").html("").hide();
    $(".login-data").show();
    $("#user-password-input").attr("name", "Password");
    $("#user-password-input").attr("required", true);
    $(".users-options-container").hide();
    $(".users-list-container").show();
}

function GoBackToGroupListFromAssign() {
    $("#contact-assign-page-counter").html("0");
    $(".group-search-for-user").val("");
    $("#search-for-assingn-contacts").submit();
    var firstId = $("#group-table").attr("first-id")
    PaginateGroupsAndPopulateTable(firstId, null, 11, null);
    $(".groups-options-container-assign").hide();
    $(".groups-list-container").show();
}

function GoBackToContactListFromAssign() {
    $("#group-assign-page-counter").html("0");
    $(".contact-search-for-group").val("");
    $("#search-for-assign-groups").submit();
    var firstId = $("#contacts-table").attr("first-id");
    PaginateContactsAndPopulateTable(firstId, null, 11, null);
    $(".contact-options-container-assign").hide();
    $(".contacts-list-container").show();
}

function setupWatcher(targetElement, watchedClass, callback) {
    const observer = new MutationObserver((mutationList, observer) => {
        for (let mutation of mutationList) {
            if (mutation.type == "attributes" && mutation.attributeName == "class" && targetElement.classList.contains(watchedClass)) {
                callback();
            }
        }
    })

    observer.observe(targetElement, { attributes: true });
    return observer;
}

function setupClassRemovalWatcher(targetElement, classToRemove, callback) {
    const observer = new MutationObserver((mutationsList, observer) => {
        for (let mutation of mutationsList) {
            if (mutation.type === 'attributes' && mutation.attributeName === 'class' && !targetElement.classList.contains(classToRemove)) {
                callback();
            }
        }
    })
    observer.observe(targetElement, { attributes: true });
    return observer;
}

function getAllUsersAndPopulateTable() {
    $.ajax({
        url: 'Home/GetAllIdentityUsers',
        type: 'GET',
        contentType: 'application/json',
        success: function (object) {
            object.users.forEach((user) => {
                let roleCell;

                if (user.role == "Owner") {
                    roleCell = `<td class="centered-cell"><span class="owner-pill">Właściciel</span></td>`;
                }
                else if (user.role == "Administrator") {
                    roleCell = `<td class="centered-cell"><span class="admin-pill">Admin</span></td>`;
                }
                else {
                    roleCell = `<td class="centered-cell"><span class="user-pill">Użytkownik</span></td>`;
                }

                var isUserCallingAdmin = document.getElementById(user.userName);

                let optionsCell

                if ((object.role == "Admin" && user.role == "User") || (object.role == "Owner" && isUserCallingAdmin == null)) {
                    optionsCell = `
                            <td class="tiny-centered-cell row-options">
                                <a href="#edit" class="icon-list user-edit">
                                    <img src="/icons/edit.png" title="Edytuj">
                                </a>
                                <a href="#delete" class="icon-list user-delete">
                                    <img src="/icons/trash.png" title="Usuń">
                                </a>
                            </td>`;
                }
                else {
                    optionsCell = `<td></td>`
                }

                var newRow = `
                    <tr>
                        <td class="tiny-centered-cell id">${user.id}</td>
                        <td>${user.name}</td>
                        <td>${user.surname}</td>
                        <td>${user.userName}</td>
                        <td class="small-cell" style="text-align: center;">${user.phoneNumber || "Brak numeru"}</td>
                        ${roleCell}
                        ${optionsCell}
                    </tr>`;

                $("#users-table tbody").append(newRow);
            });
        },
        error: function (error) {
            console.error(error.responseText);
        }
    });
}

function getRolesForUserCreation() {
    $.ajax({
        url: 'Home/GetRolesForUserCreation',
        type: 'GET',
        contentType: 'application/json',
        success: function (roles) {
            $("#role-names").empty().append(roles);
        },
        error: function (error) {
            console.error(error.responseText);
        }
    })
}

function formatPhoneNumber(phoneInput, event) {
    var phoneInputEl = document.getElementById(phoneInput);

    if (event.inputType === 'deleteContentBackward' || event.inputType === 'deleteContentForward') {
        return;
    }

    var inputValue = phoneInputEl.value;
    var cursorPosition = phoneInputEl.selectionStart;

    var sanitizedValue = inputValue.replace(/[^0-9+]/g, '');

    var formattedValue = sanitizedValue.replace(/(\S{3})/g, '$1 ').trim();
    var lengthDiff = formattedValue.length - inputValue.length;

    phoneInputEl.value = formattedValue;

    phoneInputEl.setSelectionRange(cursorPosition + lengthDiff, cursorPosition + lengthDiff);
}

function createNewUser(formData) {

    $.ajax({
        url: `/Home/DetermineUserRoleAndCreate`,
        type: 'POST',
        dataType: "json",
        data: formData,
        success: function (user) {
            let roleCell;

            if (user.role == "Owner") {
                roleCell = `<td class="centered-cell"><span class="owner-pill">Właściciel</span></td>`;
            }
            else if (user.role == "Administrator") {
                roleCell = `<td class="centered-cell"><span class="admin-pill">Admin</span></td>`;
            }
            else {
                roleCell = `<td class="centered-cell"><span class="user-pill">Użytkownik</span></td>`;
            }

            var newRow = `<tr>
                <td class="tiny-centered-cell id">${user.id}</td>
                <td class="tiny-cell">${user.name}</td>
                <td class="tiny-cell">${user.surname}</td>
                <td class="tiny-cell">${user.userName}</td>
                <td class="small-cell" style="width: 150px; text-align: center;">${user.phoneNumber || "Brak numeru"}</td>
                ${roleCell}
                 <td class="tiny-centered-cell row-options">
                    <a href="#edit" class="icon-list user-edit">
                        <img src="/icons/edit.png" title="Edytuj">
                    </a>
                    <a href="#delete" class="icon-list user-delete">
                        <img src="/icons/thrash.png" title="Usuń">
                    </a>
                </td>
            </tr>`;

            $("#users-table tbody").append(newRow);
            var desiredRow = $("#users-tabletbody tr").filter(function () {
                return $(this).find('td.id').text().trim() == user.id;
            });

            $("#user-edit-create-previous-button").trigger("click");
        },
        error: function (error) {
            var listOfErrors = `<ul style="margin: 0; padding-left: 25px; font-size: 17px; font-weight: 500;">`;

            if (typeof error.responseJSON === 'object') {
                Object.keys(error.responseJSON).forEach(function (key) {
                    var errors = error.responseJSON[key];
                    errors.forEach(function (errorMessage) {
                        listOfErrors += `<li>${errorMessage}</li>`;
                    });
                });

                listOfErrors += `</ul>
                    <button type='button' class='btn-close text-dark' onclick='CloseUserAlert()' aria-label='Close'></button>`;

                $("#status-create-edit-user").html(listOfErrors).show();
            }
            else {
                if (error.status == "403") {
                    listOfErrors += `<li>Nie można utworzyć użytkownika - brak uprawnień do nadania tej roli</li>`;
                }
                else {
                    listOfErrors += `<li>${error.responseText}</li>`;
                }

                listOfErrors += `</ul>
                    <button type='button' class='btn-close text-dark' onclick='CloseUserAlert()' aria-label='Close'></button>`;

                $("#status-create-edit-user").html(listOfErrors).show();
            }
        }
    });
}

function getUserById(userId) {
    $.ajax({
        url: `Home/DetermineUserRoleAndGetById`,
        type: 'GET',
        contentType: 'application/json',
        data: { userId: userId },
        success: function (user) {
            $("#user-role").val(user.role);
            $("#user-name-input").val(user.name);
            $("#user-surname-input").val(user.surname);
            $("#user-email-input").val(user.userName);
            $("#user-phone-number-input").val(user.phoneNumber);
            $("#user-password-input").removeAttr("name");
            $("#user-password-input").removeAttr("required");
            $(".login-data").hide();
            $("#createOrEditUserButton").html("Zatwierdź edycję");

            $.ajax({
                url: "Home/GetRolesForUserCreation",
                type: "GET",
                success: function (roles) {
                    $("#role-names").empty().append(roles);
                    $(".users-options-container .left span").html("Edycja użytkownika");
                    $("#create-edit-user-form").attr("operation", "edit");
                },
                error: function (error) {
                    console.error(error.responseText);
                }
            });

            $(".users-list-container").hide();
            $(".users-options-container").show();
        },
        error: function (error) {
            console.error(error);
        }
    });
}
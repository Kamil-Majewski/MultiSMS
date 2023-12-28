var allChoices = ["Option 1", "Option 2", "Option 3", "Option 4"]; //Fetch the options from the server
var selectedChoices = [];

function showDropdown(inputValue)
{
    var dropdownContent = document.getElementById("dropdownContent");
    dropdownContent.innerHTML = '';

    if (inputValue.trim() === '')
    {
        allChoices.forEach(choice => {
            var optionElement = document.createElement("a");
            optionElement.textContent = choice;
            optionElement.onclick = function () {
                addPill(choice);
                document.getElementById("dropdownContent").style.display = "none";
            };

            dropdownContent.appendChild(optionElement);
        });
    }
    else
    {
        allChoices.filter(choice => choice.toLowerCase().includes(inputValue.toLowerCase())).forEach(choice => {
            if (!selectedChoices.includes(choice))
            {
                var optionElement = document.createElement("a");
                optionElement.textContent = choice;
                optionElement.onclick = function () {
                    addPill(choice);
                    document.getElementById("dropdownContent").style.display = "none";
                };

                dropdownContent.appendChild(optionElement);
            }
        });
    }

    if (dropdownContent.children.length > 0)
    {
        dropdownContent.style.display = "block";
    } else
    {
        dropdownContent.style.display = "none";
    }
}

function addPill(choice)
{
    if (!selectedChoices.includes(choice))
    {
        selectedChoices.push(choice);

        var pillContainer = document.getElementById("pillContainer");
        var pill = document.createElement("div");
        pill.className = "pill";
        pill.textContent = choice;
        pill.onclick = function () {
            removePill(choice);
        };

        pillContainer.appendChild(pill);
        updateDropdown();
        document.getElementById("dropdownContent").style.display = "none";
        document.querySelector('.search-input').value = '';
    }
}

function removePill(choice)
{
    selectedChoices = selectedChoices.filter(item => item !== choice);
    var pillContainer = document.getElementById("pillContainer");
    pillContainer.innerHTML = '';

    selectedChoices.forEach(function (choice)
    {
        var pill = document.createElement("div");
        pill.className = "pill";
        pill.textContent = choice;
        pill.onclick = function ()
        {
            removePill(choice);
        };

        pillContainer.appendChild(pill);
    });

    updateDropdown();
}

function updateDropdown()
{
    var dropdownContent = document.getElementById("dropdownContent");
    dropdownContent.innerHTML = '';

    allChoices.filter(choice => !selectedChoices.includes(choice)).forEach(choice => {
        var optionElement = document.createElement("a");
        optionElement.textContent = choice;
        optionElement.onclick = function () {
            addPill(choice);
            document.getElementById("dropdownContent").style.display = "none";
        };

        dropdownContent.appendChild(optionElement);
    });

    if (dropdownContent.children.length > 0)
    {
        dropdownContent.style.display = "block";
    }
    else
    {
        dropdownContent.style.display = "none";
    }
}

document.addEventListener("click", function (event) {
    var dropdownContent = document.getElementById("dropdownContent");
    if (!event.target.matches('.search-input') && !event.target.matches('.dropdown-content a'))
    {
        dropdownContent.style.display = "none";
    }
});
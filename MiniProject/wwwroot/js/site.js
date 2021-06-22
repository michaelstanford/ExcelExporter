// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var fileName = "";

function uploadFile() {
    var fileUpload = $("#files").get(0);
    var files = fileUpload.files;

    fileName = files[0].name;

    var fileData = new FormData();

    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }

    $.ajax({
        url: '/Home/Upload',
        type: "POST",
        processData: false,
        contentType: false,
        data: fileData,
        success: function (result) {

            var jsonData = JSON.parse(result);

            var col = [];
            for (var i = 0; i < jsonData.length; i++) {
                for (var key in jsonData[i]) {
                    if (col.indexOf(key) === -1) {
                        col.push(key);
                    }
                }
            }

            var table = document.createElement("table");

            var tr = table.insertRow(-1);

            for (var i = 0; i < col.length; i++) {
                var th = document.createElement("th");
                th.innerHTML = col[i];
                tr.appendChild(th);
            }

            for (var i = 0; i < jsonData.length; i++) {

                tr = table.insertRow(-1);

                for (var j = 0; j < col.length; j++) {
                    var tabCell = tr.insertCell(-1);
                    tabCell.innerHTML = jsonData[i][col[j]];
                }
            }

            var divContainer = document.getElementById("file-data");
            divContainer.innerHTML = "";
            divContainer.appendChild(table);

            populateDropdown(col);
            
            $('#export-type-selection').show();
            $('#sort-selection').show();
            $('#export-button').show();
            $('#upload-controls').hide();

        },
        error: function (err) {
            alert(err.statusText);
        }
    });
}

function populateDropdown(values) {
    var dropdown = $('#column-headers');

    dropdown.empty();

    $.each(values, function (key, entry) {
            dropdown.append($('<option></option>').attr('value', entry).text(entry));
    });
}

function exportFile() {
    var exportAs = $('#exportAs').val();
    var sortBy = $('#column-headers').val();
    var delimiter = $('#delimiter').val();
    
    $.ajax({
        url: '/Home/Export?format=' + exportAs + '&sortedBy=' + sortBy + '&fileName=' + fileName + '&delimiter=' + delimiter,
        type: "GET",
        processData: false,
        contentType: false,
        dataType:'xml',
        complete: function(xhr, status) {
            var resultContainer = $("#preformatted");
            resultContainer.text(xhr.responseText);
        }
    });
}

let datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#companiesTable').DataTable({
        'ajax': {
            'url': '/Admin/Company/GetAll'
        },
        'columns': [
            { 'data': 'name' }
            {
                'data': 'id',
                'render': function (data) {
                    return `
                        <div class="w-75 btn-group">
                            <a href='/Admin/Company/Upsert/${data}' class="btn btn-primary mx-2">
                                <i class="bi bi-pencil"></i>
                                Edit
                            </a>
                            <a class="btn btn-danger mx-2" onclick="deleteCompany(${data})">
                                <i class="bi bi-trash"></i>
                                Delete
                            </a>
                        </div>
                    `;
                },
                'width': '15%'
            }
        ]
    });
}

function deleteCompany(id) {
    const deleteCompany = confirm('Do you really want to delete this company?');
    if (deleteCompany === true) {
        $.ajax({
            url: `/Admin/Company/Delete/${id}`,
            type: 'DELETE',
            success: function (data) {
                alert(data.message);
                if (data.success === true) {
                    datatable.ajax.reload();
                }
            }
        })
    }
}
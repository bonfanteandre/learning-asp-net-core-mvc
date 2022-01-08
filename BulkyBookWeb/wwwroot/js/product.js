let datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#productsTable').DataTable({
        'ajax': {
            'url': '/Admin/Product/GetAll'
        },
        'columns': [
            { 'data': 'title' },
            { 'data': 'isbn', 'width': '15%' },
            { 'data': 'price', 'width': '15%' },
            { 'data': 'author', 'width': '15%' },
            { 'data': 'category.name' },
            {
                'data': 'id',
                'render': function (data) {
                    return `
                        <div class="w-75 btn-group">
                            <a href='/Admin/Product/Upsert/${data}' class="btn btn-primary mx-2">
                                <i class="bi bi-pencil"></i>
                                Edit
                            </a>
                            <a class="btn btn-danger mx-2" onclick="deleteProduct(${data})">
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

function deleteProduct(id) {
    const deleteProduct = confirm('Do you really want to delete this product?');
    if (deleteProduct === true) {
        $.ajax({
            url: `/Admin/Product/Delete/${id}`,
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
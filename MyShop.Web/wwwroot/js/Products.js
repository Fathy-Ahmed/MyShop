var dtble;
var js = jQuery.noConflict(true);
js(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtble = js("#ProductTable").DataTable({
        "ajax": {
            "url": "/Admin/Product/GetData"
        },
        "columns": [
            { "data": "name" },
            { "data": "description" } ,
            { "data": "price" },
            { "data": "category" }
        ]
    });
}
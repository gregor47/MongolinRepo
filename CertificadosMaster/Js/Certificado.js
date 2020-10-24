/**Funcion que permiete obtener los datos digitados por el usuario y enviarlo a la funcion requerida */

var connection;//variable para controlar la conexion SignalR

//controla al iniciar
$(document).ready(function () {

    ///permite colocar el nombre del archivo cargado
    $('#inputFileCorreo').change(function () {
        var files = [];
        for (var i = 0; i < $(this)[0].files.length; i++) {
            files.push($(this)[0].files[i].name);
        }
        $(this).next('.custom-file-label').html(files.join(', '));
    });

    $('#inputFile').change(function () {
        var files = [];
        for (var i = 0; i < $(this)[0].files.length; i++) {
            files.push($(this)[0].files[i].name);
        }
        $(this).next('.custom-file-label').html(files.join(', '));
    });


    $("form#postProcesarFileCorreo").submit(function (e) {
        e.preventDefault();

        var fileUpload = $("#inputFileCorreo").get(0);
        var files = fileUpload.files;

        let reg = /(.*?)\.(xlsx|XLSX)$/;//permite validar tipo de archivo

        if (files.length > 0 && (files[0].name.match(reg))) {// valida si exite adjunto

            if (files[0].size <= LimiteTamArchivo) {//se valida el tamaño del archivo

                var formData = new FormData(this);

                $.ajax({
                    url: URL_ACTION_SubirFinde.toString(),
                    type: 'POST',
                    data: formData,
                    dataType: 'json',
                    success: function (d) {
                        if (d.Codigo === "01") {
                            swal("Exitoso", d.Descripcion, "success");
                            limpiar();
                        } else {
                            limpiar();
                            swal("¡Error!", "Ha ocurrido un error, intente nuevamente.", "error");
                        }
                    },
                    beforeSend: function () {

                        cargando();
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });

            } else {

                swal("¡Error!", "El tamaño del archivo excede el permitido.", "error");
            }

        } else {

            swal("¡Error!", "Por favor Seleccione un archivo válido.", "error");
        }
    });


    $("form#postProcesarFile").submit(function (e) {
        e.preventDefault();
        var fileUpload = $("#inputFile").get(0);
        var files = fileUpload.files;

        let reg = /(.*?)\.(xlsx|XLSX)$/;//permite validar tipo de archivo

        if (files.length > 0 && (files[0].name.match(reg))) {// valida si exite adjunto

            if (files[0].size <= LimiteTamArchivo) {//se valida el tamaño del archivo

                var formData = new FormData(this);

                $.ajax({
                    url: URL_ACTION_SubirMAtriculados.toString(),
                    type: 'POST',
                    data: formData,
                    dataType: 'json',
                    success: function (d) {
                        if (d.Codigo === "01") {
                            swal("Exitoso", d.Descripcion, "success");
                            limpiar();
                        } else {
                            limpiar();
                            swal("¡Error!", "Ha ocurrido un error, intente nuevamente.", "error");
                        }
                    },
                    beforeSend: function () {

                        cargando();
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });

            } else {

                swal("¡Error!", "El tamaño del archivo excede el permitido.", "error");
            }

        } else {

            swal("¡Error!", "Por favor Seleccione un archivo válido.", "error");
        }
    });

});



function cargando() {
    swal({
        title: 'Cargando',
        text: '',
        imageUrl: 'https://www.bolsadequito.com/images/loading.gif',
        imageWidth: 400,
        imageHeight: 200,
        imageAlt: 'Custom image',
        animation: false,
        showCancelButton: false,
        showConfirmButton: false,
        allowEscapeKey: false
    })
}


///Funcion limpia los datos
function limpiar() {
    $("#inputFileLabel").html("Seleccionar Archivo");
    document.getElementById('inputFile').value = '';
    $("#inputFileLabelCorreo").html("Seleccionar Archivo");
    document.getElementById('inputFileCorreo').value = '';
}



$(window).on('load',
    function() {
        $('.loader').fadeOut(500);
        $('#submitForm').validate();
    });

//$(document).ajaxStart(function() { Pace.restart(); });

function printDoc() {
    window.print();
}


$(document).on('change', '.interest_transfer', function () {

    $(document).find(".interest_bank_id").attr("disabled", !this.checked);

    if (!this.checked) $("#interest_bank_id").prop('selectedIndex', 0);
});


$(document).on('change', '#renewable', function () {

    $("#renewal_basis").attr("disabled", !this.checked);

    if (!this.checked) $("#renewal_basis").prop('selectedIndex', 0);
});


$(document).on('click', '.nav-tabs a', function(e){
    e.preventDefault();
    $(".tab-pane").addClass('hidden').removeClass('active');
    //alert($(this).attr("href"));
    $($(this).attr("data-href")).removeClass('hidden').addClass('active');
});

$(document).on('click', '.edit-button', function (e) {
    e.preventDefault();
    if (this.tagName !== "A" ) {

        var modal = document.querySelector("#modal-default");

        if (!modal) return;

        $(modal).modal();
    } else {
        var url = $(this).attr('href');
        var m = document.querySelector("#modal-update");
        if( !m ) return;
        $(m).modal();
        
        $.ajax({
            type: "GET",
            url: url,
            success: function (res) {
                $(m).find(".modal-content").html(res);

                $('#submitForm').validate();
            }
        });
    }
    return false;
});

    $(document).on('click','.edit-v2',function (e) {
    e.preventDefault();
    $.ajax({
        type: "GET",
        url: this.href,
        success: function (res) {
            $("#modal-default").modal('show');
            for(var x in res.data){

                if (res.data.hasOwnProperty(x))
                {
                    $("#" + x).val(res.data[x]);
                }
            }
        }
    });
});



$(document).on('blur', '.search-customer', function () {
    var el = $(this);
    el.addClass("img-loader");
    $.ajax({
        type: "GET",
        url: "/Customer/SearchCustomer/" + encodeURIComponent(this.value),
        success: function (res) {
            el.removeClass("img-loader");
            if (res !== "0") $(el).closest(".modal-content").html(res);
        }
    })
});


function formLoader(form) {
    var cCheck = form.querySelector(".loading-pro");
    if (cCheck) $(cCheck).fadeIn();
    else {
        var loading = document.createElement("div");
        $(loading).addClass("formLoader");
        $(loading).addClass("loading-pro");
        var topBar = document.createElement("div");
        topBar.className = "load-bar absolute top-left";
        topBar.innerHTML = "<div class=\"bar\"></div><div class=\"bar\"></div><div class=\"bar\"></div><div class=\"bar\"></div>";
        loading.appendChild(topBar);
        var loader2 = document.createElement("div");
        loader2.className = "loader-new didHide";
        loading.appendChild(loader2);
        setInterval(function (res) {
            $(topBar).slideToggle();
            $(loader2).slideToggle();
        }, 9000);
        var innerLoad = document.createElement("div");
        $(innerLoad).addClass("formLoading");
        loading.appendChild(innerLoad);

        form.insertBefore(loading, form.firstChild);

        $(loading).fadeIn();

        cCheck = loading;
    }
    return cCheck;
}


$(document).on('submit',
    '#submitForm',
    function(e) {
        e.preventDefault();
        var form = $(this);

        if (!form.valid()) return false;

        var button = $("#createBtn");
        button.button('loading');
        var ld = formLoader(this);
        $.ajax({
            url: form.attr('action'),
            type: "POST",
            data: form.serialize()
        }).done(function(response) {
            // button loading
            button.button('reset');

            window.location.reload();
            $('.myModal').modal("hide");
            $('.myModal').on('hidden.bs.modal',
                function(e) {
                    $('.loader').fadeIn(500);
                    window.location.reload();
                });

            // reload the manage member table
//            form[0].reset();
            /* $('#add-messages').html('<div class="alert alert-success">' +
                 '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                 '<strong><i class="glyphicon glyphicon-ok-sign"></i></strong> '
                 + " Record successfully saved" + '</div>');
            
             $(".alert-success").delay(500).show(10, function () {
                 $(this).delay(3000).hide(10, function () {
                    $(this).remove();
                 });
             }); // /.alert*/
        })
            .fail(function(error) {
                $(ld).hide();
                button.button('reset');
                $('#add-messages').html('<div class="alert alert-danger">' +
                    '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                    '<strong><i class="glyphicon glyphicon-ok-sign"></i></strong> ' +
                    "Unable to save record" +
                    '</div>');

                $(".alert-danger").delay(500).show(10,
                    function() {
                        $(this).delay(3000).hide(10,
                            function() {
                                $(this).remove();
                            });
                    }); // /.alert
            });
    });


$(function() {
    $('.select2').select2();
    $('#addButton').click(function() {
        $('.myModal').modal();
        $('#id').val(0);
    });


    $('.myModal').on('hidden.bs.modal',
        function(e) {
            $(this).find("#reset").click();
            $('.form-control').removeClass('input-validation-error');
            $('.field-validation-valid').text("");
        });

    $('#btn-upload-photo').on('click',
        function() {
            $(this).siblings('#file').trigger('click');
        });

    $(document).find('[data-toggle="tooltip"]').tooltip();

    $('.printBtn').click(function() {
        printDoc();
    });

    $('.js-edit').on('click',
        function(e) {
            e.preventDefault();
            $('#addModal').modal("show");

        });


    $('.datepicker').datepicker({
        autoclose: true,
        todayHighlight: true
    });

    $('.clear_text').val("");

    //submit  form

    // submit of edit categories form
    $("#editForm").submit(function(e) {
        e.preventDefault();
        var form = $(this);

        if (!form.valid()) return false;

        // button loading
        var btn = $("#editBtn");
        btn.button('loading');
        $.ajax({
            url: form.attr('action'),
            type: 'PUT',
            data: form.serialize()
        }).done(function(response) {
            // button loading
            btn.button('reset');
            // reload the manage member table
            table.destroy();
            myFunc();

            $('#edit-messages').html('<div class="alert alert-success">' +
                '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                '<strong><i class="glyphicon glyphicon-ok-sign"></i></strong> ' +
                "Record successfully updated" +
                '</div>');

            $(".alert-success").delay(500).show(10,
                function() {
                    $(this).delay(3000).hide(10,
                        function() {
                            $(this).remove();
                        });
                }); // /.alert
        }).fail(function(error) {
            btn.button('reset');
            $('#edit-messages').html('<div class="alert alert-danger">' +
                '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                '<strong><i class="glyphicon glyphicon-ok-sign"></i></strong> ' +
                "Unable to update record" +
                '</div>');

            $(".alert-danger").delay(500).show(10,
                function() {
                    $(this).delay(3000).hide(10,
                        function() {
                            $(this).remove();
                        });
                }); // /.alert
        });
    });


});
/*


// delete button click
$(document).on("click", ".js-delete", function () {
    var deleteUrl = $(this).attr("data-url");
    var button = $(this);
    deleteWithUrl(deleteUrl, button, table);
});

function deleteWithUrl(deleteUrl, button, table) {
    var confirmButton = $('button.confirm');
    swal({
        title: "Are you sure?",
        text: "You will not be able to recover this data!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#007B00",
        confirmButtonText: "Ok!",
        closeOnConfirm: false
    }, function () {
        confirmButton.button('loading');
        $.ajax({
            url: deleteUrl,
            data: { _token: token },
            method: 'DELETE'
        })
            .done(function (response) {
                confirmButton.button('reset');
                swal({
                    title: "Deleted!",
                    text: "Record  has been deleted.",
                    type: "success",
                    confirmButtonColor: "#007B00",
                    confirmButtonText: "Close"
                });
                // reload the manage member table
                var tr = button.parents("tr");
                table.rows(tr).remove().draw(false);
            })
            .fail(function (error) {
                confirmButton.button('reset');
                swal({
                    title: "Not Deleted!",
                    text: "Record is not deleted please try again later.",
                    type: "info",
                    confirmButtonColor: "#ff3f71",
                    confirmButtonText: "Ok ,Close"
                });
            });

    });
}
*/


$(document).on('click',
    '.js-delete',
    function(e) {
        e.preventDefault();

        var button = $(this);
        var url = button.attr("href");
        bootbox.confirm("Are you sure you want to delete this Record?",
            function(result) {
                if (result) {
                    $('.bootbox-accept').button('loading');
                    $.ajax({
                        url: url,
                        method: 'DELETE',
                        success: function() {
                            location.reload();
                        }
                    });
                    return false;
                }
                return true;
            });

    });

function showLoader() {
    // modal spinner
    $('.modal-loading').removeClass('div-hide');
    // modal result
    $('.edit-result').addClass('div-hide');
    //modal footer
    var footer = $(".editFooter");
    footer.addClass('div-hide');
}

function hideLoader() {
    // modal spinner
    $('.modal-loading').addClass('div-hide');
    // modal result
    $('.edit-result').removeClass('div-hide');
    //modal footer
    $(".editFooter").removeClass('div-hide');
}


var showToast = function(message, timeOut, close) {
    // Display a warning toast, with no title
    toastr.options = {
        "closeButton": close,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-center",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": timeOut.toString(),
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    toastr.error(message, { timeOut: timeOut });
};
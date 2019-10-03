
$(document).on('change', '#interest_transfer', function () {

    $("#interest_bank_id").attr("disabled", !this.checked);

    if (!this.checked) $("#interest_bank_id").prop('selectedIndex', 0);
});


$(document).on('change', '#renewable', function () {

    $("#renewal_basis").attr("disabled", !this.checked);

    if (!this.checked) $("#renewal_basis").prop('selectedIndex', 0);
});


$(function () {
    //$('.select2').select2();
    //$(".auto-clear").val("");
});

var val = null;

$(document).on('click', '.edit-button', function (e) {
    e.preventDefault();
    var url = $(this).attr('href');
    var modal = document.querySelector("#modal-default"); 

    if (!modal) return;

    $(modal).modal();


    var cnt = modal.querySelector(".modal-content");

    if (!cnt) return;

    if (val === null ) val = $(cnt).html();

   ld = formLoader(cnt);

    $(modal).on('submit', 'form', function (e) {
        e.preventDefault();
        if ($(this).hasClass("auto-sub")) {
            ld = formLoader(cnt);
            this.submit();
        }else save();
    });


    function save() {
        var form = $(modal).find("form");

        form.validate();

        ld = formLoader(cnt);
        $.ajax({
            type: "POST",
            url: form.attr("action"),
            data: form.serialize(),
            success: function (res) {
                if (res === "1") {
                    window.location.reload();
                } else {
                    $(ld).hide();
                    $(cnt).html(res);
                }
            }
        });
        return false;
    }

    

    //showLoader();




    if (this.tagName !== "A" && val !== null) {
        
            $(cnt).html(val);
            $('.select2').select2();
       
        $(ld).hide();
    } else
        $.ajax({
            type: "GET",
            url: url,
            success: function (res) {
                $(cnt).html(res);
                $(ld).hide();
                $('#select2').select2();
            }
        });
    return false;
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
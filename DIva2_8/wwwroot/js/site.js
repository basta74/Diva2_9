const days = ['ne', 'po', 'út', 'st', 'čt', 'pá', 'so'];
const msgTypes = ['info', 'success', 'danger', 'warning', 'error', 'primary'];
var daySecond = 60 * 60 * 24;

Number.prototype.pad = function (size, what) {
    var s = String(this);
    while (s.length < (size || 2)) { s = what + s; }
    return s;
}

$(function () {

    var placeholderElement = $('#modal-placeholder');

    $(document).on('click', 'button[data-toggle="ajax-modal"]', function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });

    placeholderElement.on('click', '[data-save="modal"]', function (event) {

        //console.log("data-save=modal");

        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();

        $.post(actionUrl, dataToSend).done(function (data) {
            var newBody = $('.modal-body', data);
            placeholderElement.find('.modal-body').replaceWith(newBody);

            var isValid = newBody.find('[name="IsValid"]').val() == 'True';
            if (isValid) {
                placeholderElement.find('.modal').modal('hide');
                location.reload();
            }
        });
    });

    $('[data-toggle="tooltip"]').tooltip();


    $(document).on('click', 'table.js-td-odkaz tr td', function (event) {

        var id = $(this).closest("tr").data("id");
        var url = $(this).closest("table").data("url");
        window.location.href = url + "/" + id;

    });

    $(document).on('click', 'table.js-td-odkaz-m tr td', function (event) {

        var id = $(this).closest("tr").data("id");
        var url = $(this).closest("table").data("url");
        var urlM = url + "/" + id;
        $.get(urlM).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });

    });
});

function writeMesssages(messages) {
    var str = "";
    messages.forEach(function (msg) {
        str += '<div class="alert alert-' + msgTypes[msg.type] + '" role="alert">' + msg.text + '</div>';
    });

    return str;

}

function writeDate(unix) {

    var date = new Date(unix * 1000);
    
    var d = date.getDate();
    var m = date.getMonth() + 1; //Month from 0 to 11
    //var y = date.getFullYear();
    var y = date.getYear() - 100;


    return (d <= 9 ? '0' + d : d) + '.' + (m <= 9 ? '0' + m : m) + '.' + y;


    return str;

}

/*
window.addEventListener('load', function () {
    // At first, let's check if we have permission for notification
    // If not, let's ask for it
/*
if (window.Notification && Notification.permission !== "granted") {
        Notification.requestPermission(function (status) {
            if (Notification.permission !== status) {
                Notification.permission = status;
            }
        });
    } 


    console.log(Notification.permission);

    var button = document.getElementById('noti-enable');

    button.addEventListener('click', function () {
        console.log("click");
        // If the user agreed to get notified
        // Let's try to send ten notifications
        if (window.Notification && Notification.permission === "granted") {
            console.log("povoleno");
            var i = 0;
            // Using an interval cause some browsers (including Firefox) are blocking notifications if there are too much in a certain time.
            var interval = window.setInterval(function () {
                // Thanks to the tag, we should only see the "Hi! 9" notification 
                var n = new Notification("Hi! " + i, { tag: 'soManyNotification' });
                if (i++ == 9) {
                    window.clearInterval(interval);
                }
            }, 200);
        }

        // If the user hasn't told if he wants to be notified or not
        // Note: because of Chrome, we are not sure the permission property
        // is set, therefore it's unsafe to check for the "default" value.
        else if (window.Notification && Notification.permission !== "denied") {
            console.log("neni zakazano");
            Notification.requestPermission(function (status) {
                // If the user said okay
                if (status === "granted") {
                    var i = 0;
                    // Using an interval cause some browsers (including Firefox) are blocking notifications if there are too much in a certain time.
                    var interval = window.setInterval(function () {
                        // Thanks to the tag, we should only see the "Hi! 9" notification 
                        var n = new Notification("Hi! " + i, { tag: 'notificationLesson' });
                        if (i++ == 9) {
                            window.clearInterval(interval);
                        }
                    }, 200);



                }

                // Otherwise, we can fallback to a regular modal alert
                else {
                    alert("Hi!");
                }
            });
        }

        // If the user refuses to get notified
        else {
            // We can fallback to a regular modal alert
            console.log("polveno");
            alert("Hi!");
        }
    });
});

/**/
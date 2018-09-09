﻿$('#download-more-notifications').click(function () {
    const downloadNotificationsAmount = 5;
    downloadNotifications(downloadNotificationsAmount);
});

function downloadNotifications(takeAmount) {
    let token = $('input[name="__RequestVerificationToken"]', $('#antiForgeryToken')).val();
    let amountOfNotificationsNow = $('#notificationsTable>tr')
        .filter((idx, item) => $(item).css('display') !== 'none').length;
    let notificationReceiverUserName = $('input[name="notificationReceiver"]').val();
    $.post('/Notification/GetNotifications',
        {
            '__RequestVerificationToken': token,
            'skipAmount': amountOfNotificationsNow,
            'takeAmount': takeAmount,
            'receiverUserName': notificationReceiverUserName
        },
        function (notifications) {
            notifications.forEach(addNotification);
        }).fail(
            function (errorObj) {
                console.error(errorObj.responseJSON.message);
            });
}

function addNotification(notification) {
    let tr = $('<tr/>');

    tr.append($('<td/>', { text: notification.title }));

    let notificationSource = $('<a/>', { href: notification.source, text: 'link' });
    let sourceLinkTd = $('<td/>').append(notificationSource);
    tr.append(sourceLinkTd);

    let formattedDate = new Date(notification.createdDate);
    tr.append($('<td/>', { text: formattedDate.toLocaleString().replace(',', '') }));

    let deleteLink = $('<a/>', { text: 'Delete', class: 'delete-notification' })
        .data('notificationid', notification.id).css('cursor', 'pointer').click(deleteNotificationPost);
    tr.append($('<td/>').append(deleteLink));
    $('#notificationsTable').append(tr);
}
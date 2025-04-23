$(document).ready(function() {
    // Initialize SignalR connection
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .build();

    // Handle incoming notifications
    connection.on("ReceiveNotification", function(message) {
        // Display notification (e.g., alert or UI update)
        alert(message);
        // Optional: Update UI (e.g., append to a notification list)
        $("#notificationList").append(`< li class= "list-group-item" >${ message}</ li >`);
    });

// Start connection
connection.start().catch(function(err) {
    console.error(err.toString());
});
});
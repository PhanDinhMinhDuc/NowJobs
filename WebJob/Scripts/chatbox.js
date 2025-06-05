//function loadMessages() {
//    $.ajax({
//        url: '/Message/GetMessages',
//        type: 'GET',
//        success: function (response) {
//            var messagesContainer = document.getElementById("messagesContainer");
//            messagesContainer.innerHTML = "";

//            if (response.success !== false) {
//                response.forEach(function (message) {
//                    var messageElement = document.createElement("div");
//                    messageElement.className = (message.Sender === "Bạn") ? "message user" : "message admin";
//                    messageElement.innerHTML = `<strong>${message.Sender}:</strong> ${message.Content}`;
//                    messagesContainer.appendChild(messageElement);
//                });

//                messagesContainer.scrollTop = messagesContainer.scrollHeight;
//            }
//        }
//    });
//}

//setInterval(loadMessages, 3000);

//document.addEventListener("DOMContentLoaded", function () {
//    var chatIcon = document.getElementById("chatIcon");
//    var chatBox = document.getElementById("chatBox");
//    var sendMessageForm = document.getElementById("sendMessageForm");
//    var textarea = sendMessageForm.querySelector("textarea[name='content']");

//    chatIcon.onclick = function () {
//        if (chatBox.style.display === "none" || chatBox.style.display === "") {
//            chatBox.style.display = "block";
//            loadMessages();
//        } else {
//            chatBox.style.display = "none";
//        }
//    };

//    sendMessageForm.onsubmit = function (event) {
//        event.preventDefault();
//        var content = textarea.value.trim();
//        if (content === "") return;

//        $.ajax({
//            url: '/Message/SendMessage',
//            type: 'POST',
//            data: { content: content },
//            success: function (response) {
//                if (response.success) {
//                    var messagesContainer = document.getElementById("messagesContainer");

//                    var newMessage = document.createElement("div");
//                    newMessage.className = "message user";
//                    newMessage.innerHTML = `${response.message}`;
//                    messagesContainer.appendChild(newMessage);

//                    textarea.value = "";
//                    messagesContainer.scrollTop = messagesContainer.scrollHeight;
//                } else {
//                    alert("Gửi tin nhắn thất bại. Vui lòng đăng nhập!");
//                }
//            }
//        });
//    };
//    document.addEventListener("click", function (event) {
//        var isClickInsideChatBox = chatBox.contains(event.target);
//        var isClickOnChatIcon = chatIcon.contains(event.target);

//        if (!isClickInsideChatBox && !isClickOnChatIcon) {
//            chatBox.style.display = "none";
//        }
//    });
//});



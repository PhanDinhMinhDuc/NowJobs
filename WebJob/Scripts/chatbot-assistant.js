const toggleBtn = document.getElementById('chat-toggle');
const popup = document.getElementById('chat-popup');
const closeBtn = popup.querySelector('.close-btn');
const chatBody = document.getElementById('chat-body');
const chatInput = document.getElementById('chat-input');
const sendBtn = document.getElementById('send-btn');
const systemPrompt = `Bạn là một trợ lý chuyên về thị trường tuyển dụng và các vị trí công việc/ngành nghề. Nếu khi người dùng hỏi về các job liên quan hiện có thì hãy lấy ở
        file json ở dưới để trả lời, còn nếu không có thì bạn có thể tự tư vấn giúp tôi
                [
          {
            "id": 1,
            "title": "Coder backend",
            "val1": 5,
            "val2": 3,
            "val3": null,
            "val4": 3,
            "val5": 1,
            "val6": null,
            "val7": null,
            "val8": 1,
            "position": "backend",
            "description": "dsadsadasdsadsa",
            "notes": null,
            "requirements": "dsadsadsadas",
            "status": 1
          },
          {
            "id": 2,
            "title": "Front-end dev",
            "val1": 6,
            "val2": 4,
            "val3": null,
            "val4": 4,
            "val5": 2,
            "val6": null,
            "val7": null,
            "val8": 1,
            "position": "Developer",
            "description": "Môt tả công việc",
            "notes": null,
            "requirements": "Yêu cầu công việc",
            "status": 1
          }
        ]

`;

toggleBtn.addEventListener('click', () => { popup.style.display = 'flex'; chatInput.focus(); });
closeBtn.addEventListener('click', () => { popup.style.display = 'none'; });

function appendMessage(text, cls) {
    const msg = document.createElement('div'); msg.classList.add('message', cls); msg.textContent = text;
    chatBody.appendChild(msg); chatBody.scrollTop = chatBody.scrollHeight;
}

function sendMessage() {
    const text = chatInput.value.trim(); if (!text) return; appendMessage(text, 'user'); chatInput.value = '';
    callChatGPT(text);
}
sendBtn.addEventListener('click', sendMessage);
chatInput.addEventListener('keypress', e => { if (e.key === 'Enter') sendMessage(); });

async function callChatGPT(userPrompt) {
    appendMessage('Đang xử lý...', 'bot');
    try {
        const res = await fetch('https://api.openai.com/v1/chat/completions', {
            method: 'POST', headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer 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'
            }, body: JSON.stringify({
                model: 'gpt-3.5-turbo', messages: [
                    { role: 'system', content: systemPrompt }, { role: 'user', content: userPrompt }
                ], max_tokens: 512
            })
        });
        const data = await res.json(); const lastBot = chatBody.querySelector('.bot:last-child');
        if (lastBot && lastBot.textContent === 'Đang xử lý...') lastBot.remove();
        const reply = data.choices?.[0]?.message?.content || 'Không có phản hồi.';
        appendMessage(reply, 'bot');
    } catch (err) {
        console.error(err);
        const lastBot = chatBody.querySelector('.bot:last-child');
        if (lastBot && lastBot.textContent === 'Đang xử lý...') lastBot.remove();
        appendMessage('Lỗi kết nối API.', 'bot');
    }
}
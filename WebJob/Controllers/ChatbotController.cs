using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using WebJob.Filters;
namespace WebJob.Controllers
{
    [CustomAuthorize(Roles = "Candidate")]
    public class ChatbotController : Controller
    {
        private readonly string _apiKey = ConfigurationManager.AppSettings["OpenAI_ApiKey"];
        private readonly string _apiUrl = "https://api.openai.com/v1/chat/completions";

        [HttpPost]
        public async Task<JsonResult> SendMessage(string message)
        {
            try
            {
                var systemPrompt = @"Bạn là một trợ lý chuyên về thị trường tuyển dụng và các vị trí công việc/ngành nghề. Nếu khi người dùng hỏi về các job liên quan hiện có thì hãy lấy ở
                file json ở dưới để trả lời, còn nếu câu hỏi quá dài thì đưa ra tên việc của công ty nào rồi đưa link luôn, đây là link ""localhost:44381/chi-tiet/chuyen-vien-marketing-p{0}"" sau khi tư vấn hãy lấy ID của JobID bỏ vào link gán https rồi đưa cho ứng viên phân bố đẹp 1 xíu, 
                [
                  {
                    ""JobID"": 6,
                    ""JobTitle"": ""Nhân viên kinh doanh thị trường"",
                    ""Company"": Công ty Cổ phần Phát triển Bất động sản Á Châu,
                    ""LocationID"": 10,
                    ""JobDescription"": ""Mô tả công việc: Tiếp cận khách hàng tại khu vực phụ trách,"",
                    ""JobBenefits"": ""Thưởng doanh số, phụ cấp đi lại, cơ hội thăng tiến."",
                    ""JobRequirements"": ""Giao tiếp tốt, có xe máy, ưu tiên ứng viên có kinh nghiệm sale."",
                    ""IsActive"": true,
                  },
                  {
                    ""JobID"": 7,
                    ""JobTitle"": ""Chuyên viên Marketing"",
                    ""CompanyID"": Công ty Cổ phần Phát triển Bất động sản Á Châu,
                    ""LocationID"": 10,
                    ""JobDescription"": ""Chuyên viên Marketing sẽ chịu trách nhiệm xây dựng và triển khai các chiến lược ."",
                    ""JobBenefits"": ""Mức lương cạnh tranh: Thỏa thuận theo năng lực và kinh nghiệm thực tế. Chế độ nghỉ phép: 12 ngày phép/năm."",
                    ""JobRequirements"": ""Chúng tôi tìm kiếm những ứng viên có tinh thần học hỏi, sáng tạo và nhiệt huyết trong lĩnh vực marketing.."",
                    ""IsActive"": true,
                  }
                ]
                "; // Your system prompt here

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                    var requestData = new
                    {
                        model = "gpt-3.5-turbo",
                        messages = new[]
                        {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = message }
                    },
                        max_tokens = 512
                    };

                    var content = new StringContent(
                        JsonConvert.SerializeObject(requestData),
                        Encoding.UTF8,
                        "application/json");

                    var response = await client.PostAsync(_apiUrl, content);
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<dynamic>(responseString);

                    return Json(new
                    {
                        success = true,
                        reply = responseData.choices[0].message.content.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    details = ex.Message
                });
            }
        }
    }
}

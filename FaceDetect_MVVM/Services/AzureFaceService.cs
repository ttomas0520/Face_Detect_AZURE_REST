using FaceDetect_MVVM.Core.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace FaceDetectHF.Services
{
    class AzureFaceService
    {
        const string SUBSCRIPTION_KEY = "6e071561d2a24316aa51a4215fbb3598";
        const string ENDPOINT = "https://facedetecthf.cognitiveservices.azure.com/";

        /// <summary>
        /// Az Azure Api-hoz való kapcsolódásért felelős függvény
        /// </summary>
        /// <param name="base64">A kiválasztott kép base64 byte tömbbe alakított alakja</param>
        /// <returns>A detektált arcok</returns>
        public async Task<List<DetectedFace>> MakeRequest(byte[] base64)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SUBSCRIPTION_KEY);

            queryString["returnFaceId"] = "true";
            queryString["returnFaceLandmarks"] = "false";
            queryString["returnFaceAttributes"] = "age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";
            queryString["recognitionModel"] = "recognition_01";
            queryString["returnRecognitionModel"] = "false";
            queryString["detectionModel"] = "detection_01";
            queryString["faceIdTimeToLive"] = "86400";
            var uri = ENDPOINT + "face/v1.0/detect?" + queryString;

            HttpResponseMessage response;

            using (var content = new ByteArrayContent(base64))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                if (!response.IsSuccessStatusCode) throw new System.Exception("Connection issue, code" + response.StatusCode);
                var json = await response.Content.ReadAsStringAsync();
                List<DetectedFace> result = JsonConvert.DeserializeObject<List<DetectedFace>>(json);
                return result;
            }
        }
    }
}

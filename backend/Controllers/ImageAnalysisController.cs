using Microsoft.AspNetCore.Mvc;
using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

/*
     [ApiController]......
     This is an attribute in ASP.NET Core that marks the class  
     ImageAnalysisController as an API controller. It enables various features 
     like automatic model binding and validation for incoming HTTP requests.

    [Route("api/ImageAnalysis")].....
     This attribute specifies the base route for the controller. [controller] is a placeholder 
     that will be replaced with the controller's name, so requests to this controller will be routed to routes 
     like /api/ImageAnalysis.
*/
[ApiController]
[Route("api/ImageAnalysis")]
public class ImageAnalysisController : ControllerBase
{
    //This attribute specifies that the AnalyzeImage method should respond to HTTP GET requests. In this case, it defines an endpoint that will perform image analysis.
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AnalyzeImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Invalid File");
        }

        try
        {
            string? visionKey = Environment.GetEnvironmentVariable("VISION_KEY");

            if (string.IsNullOrEmpty(visionKey))
            {
                // Handle the case where the key is missing or empty, e.g., throw an exception or provide a default value.
                return BadRequest("VISION_KEY is missing or empty.");
            }

            var serviceOptions = new VisionServiceOptions(
                Environment.GetEnvironmentVariable("VISION_ENDPOINT"),
                new AzureKeyCredential(visionKey)); ;

            var tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageSource = VisionSource.FromFile(tempFilePath);

            // using var imageSource = VisionSource.FromUrl(
            //     new Uri("https://i.gyazo.com/83a82ffd9c47993e5c6761867637b4d1.png"));

            /*
                var analysisOptions = ...: 
                Here, an analysisOptions object is created to specify the parameters for the image analysis. 
                It sets the features to analyze (caption and text), the language for analysis (English), and enables gender-neutral caption generation.
            */
            var analysisOptions = new ImageAnalysisOptions()
            {
                Features = ImageAnalysisFeature.Caption | ImageAnalysisFeature.Text,
                Language = "en",
                GenderNeutralCaption = true
            };

            /*
                 This code initializes an ImageAnalyzer using the service options, image source, and analysis options defined earlier. 
                 It then asynchronously performs the image analysis and stores the result in the result variable.
            */
            using var analyzer = new ImageAnalyzer(serviceOptions, imageSource, analysisOptions);
            var result = await analyzer.AnalyzeAsync();

            /*
                This section checks the result of the image analysis. If the analysis was successful (ImageAnalysisResultReason.Analyzed) and a caption was detected (result.Caption != null), 
                it creates a response containing the caption content and confidence level and returns it as an HTTP 200 OK response.
            */
            if (result.Reason == ImageAnalysisResultReason.Analyzed && result.Caption != null)
            {
                var captionResult = new
                {
                    Caption = result.Caption.Content,
                    Confidence = result.Caption.Confidence
                };
                return Ok(captionResult);
            }

            /*
                This section handles exceptions that may occur during the image analysis process. If an exception occurs, 
                it returns an HTTP 500 Internal Server Error response with an error message. If the image analysis fails for other reasons, it returns a BadRequest response with error details.
            */
            else
            {
                var errorDetails = ImageAnalysisErrorDetails.FromResult(result);
                return BadRequest(new
                {
                    ErrorReason = errorDetails.Reason,
                    ErrorCode = errorDetails.ErrorCode,
                    ErrorMessage = errorDetails.Message
                });
            }
        }
        catch (Exception e)
        {
            return StatusCode(500, new { ErrorMessage = e.Message });
        }
    }
}

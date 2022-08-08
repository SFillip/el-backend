using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using EarthLat.Backend.Core.BusinessLogic;
using EarthLat.Backend.Core.Models;
using EarthLat.Backend.Core.Extensions;
using EarthLat.Backend.Core.Dtos;
using EarthLat.Backend.Function.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using EarthLat.Backend.Core.JWT;

namespace EarthLat.Backend.Function
{
    public class StatisticFunctions
    {
        private readonly StatisticService statisticService;
        private readonly JwtValidator validator;
        private readonly string INVALID_HEADER_MESSAGE = "invalid Headers";
        private readonly string NO_DATA_FOUND_MESSAGE = "no data found";
        public StatisticFunctions(StatisticService statisticService, JwtValidator validator)
        {
            this.statisticService = statisticService;
            this.validator = validator;
        }

        //[Function(nameof(Authenticate))]
        //[OpenApiRequestBody("applicaton/json", typeof(UserCredentials), Description = "Contains username and password of the user who attempts to login")]
        //[OpenApiOperation(operationId: nameof(Authenticate), tags: new[] { "Frontend API" }, Summary = "Authenticate the user")]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDto), Description = "The user matching the credentials")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request response.", Description = "Request could not be processed.")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Resource not found.")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Unauthorized, Description = "Unauthorized access.")]
        //public async Task<ActionResult<UserDto>> Authenticate(
        //[HttpTrigger(AuthorizationLevel.Function, "post", Route = "Authenticate")] HttpRequestData request)
        //{
        //    try
        //    {
        //        string requestBody = await request.GetRequestBody();
        //        var credentials = JsonConvert.DeserializeObject<UserCredentials>(requestBody);
        //        var userDto = statisticService.Authenticate(credentials).Result;
        //        return (userDto == null)
        //            ? new UnauthorizedObjectResult("Username or Password not found")
        //            : new OkObjectResult(userDto);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.StackTrace);
        //        return new NotFoundResult();
        //    }
        //}

        //[Function(nameof(GetSendTimes))]
        //[OpenApiOperation(operationId: nameof(GetSendTimes), tags: new[] { "Frontend API" }, Summary = "Get the sending times of a station")]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<BarChartDto>), Description = "The start- and endtime(s) of a/all stations sending activity on a certain day")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request response.", Description = "Request could not be processed.")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Resource not found.")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Unauthorized, Description = "Unauthorized access.")]
        //public async Task<ActionResult<BarChartDto>> GetSendTimes(
        //[HttpTrigger(AuthorizationLevel.Function, "get", Route = "SendTimes")] HttpRequestData request)
        //{
        //    try
        //    {
        //        validator.Validate(request);
        //        if (!validator.IsValid)
        //        {
        //            return new UnauthorizedResult();
        //        }
        //        if (!request.Headers.AreValidHeaders())
        //        {
        //            return new NotFoundObjectResult(INVALID_HEADER_MESSAGE);
        //        }
        //        var (referenceDateTime, timezoneOffset) = request.Headers.GetHeaders();
        //        var sendTimes = await statisticService.GetSendTimes(
        //            validator.Id,
        //            referenceDateTime,
        //            int.Parse(timezoneOffset));
        //        if (sendTimes == null)
        //            return new NotFoundObjectResult(NO_DATA_FOUND_MESSAGE);
        //        return new OkObjectResult(sendTimes);
        //    }
        //    catch (Exception e)
        //    {
        //        return new ConflictObjectResult(e.Message);
        //    }
        //}

        //[Function(nameof(GetImagesPerHour))]
        //[OpenApiOperation(operationId: nameof(GetImagesPerHour), tags: new[] { "Frontend API" }, Summary = "Get the sending times of a station")]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LineChartDto>), Description = "The uploaded images per hour of a/all station(s) on a certain day")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request response.", Description = "Request could not be processed.")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Resource not found.")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Unauthorized, Description = "Unauthorized access.")]
        //public async Task<ActionResult<LineChartDto>> GetImagesPerHour(
        //[HttpTrigger(AuthorizationLevel.Function, "get", Route = "ImagesPerHour")] HttpRequestData request)
        //{
        //    try
        //    {
        //        validator.Validate(request);
        //        if (!validator.IsValid)
        //        {
        //            return new UnauthorizedResult();
        //        }
        //        if (!request.Headers.AreValidHeaders())
        //        {
        //            return new NotFoundObjectResult(INVALID_HEADER_MESSAGE);
        //        }
        //        var (referenceDateTime, timezoneOffset) = request.Headers.GetHeaders();
        //        var sendTimes = await statisticService.GetImagesPerHour(
        //            validator.Id,
        //            referenceDateTime,
        //            int.Parse(timezoneOffset));
        //        if (sendTimes == null)
        //            return new NotFoundObjectResult(NO_DATA_FOUND_MESSAGE);
        //        return new OkObjectResult(sendTimes);
        //    }
        //    catch (Exception e)
        //    {
        //        return new ConflictObjectResult(e.Message);
        //    }
        //}

        //[Function(nameof(GetBrightnessValuesPerHour))]
        //[OpenApiOperation(operationId: nameof(GetBrightnessValuesPerHour), tags: new[] { "Frontend API" }, Summary = "Get the sending times of a station")]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LineChartDto>), Description = "The average brightness of the uploaded images of a/all station(s) per")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad Request response.", Description = "Request could not be processed.")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Resource not found.")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Unauthorized, Description = "Unauthorized access.")]
        //public async Task<ActionResult<LineChartDto>> GetBrightnessValuesPerHour(
        //[HttpTrigger(AuthorizationLevel.Function, "get", Route = "BrightnessValues")] HttpRequestData request)
        //{
        //    try
        //    {
        //        validator.Validate(request);
        //        if (!validator.IsValid)
        //        {
        //            return new UnauthorizedResult();
        //        }
        //        if (!request.Headers.AreValidHeaders())
        //        {
        //            return new NotFoundObjectResult(INVALID_HEADER_MESSAGE);
        //        }
        //        var (referenceDateTime, timezoneOffset) = request.Headers.GetHeaders();
        //        var brightnessValues = await statisticService.GetBrightnessValuesPerHour(
        //            validator.Id,
        //            referenceDateTime,
        //            int.Parse(timezoneOffset));
        //        if (brightnessValues == null)
        //            return new NotFoundObjectResult(NO_DATA_FOUND_MESSAGE);
        //        return new OkObjectResult(brightnessValues);
        //    }
        //    catch (Exception e)
        //    {
        //        return new ConflictObjectResult(e.Message);
        //    }
        //}
    }
}
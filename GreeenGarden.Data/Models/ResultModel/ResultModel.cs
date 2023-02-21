﻿namespace GreeenGarden.Data.Models.ResultModel
{
    public class ResultModel
    {
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public object? Data { get; set; }
        public object? ResponseFailed { get; set; }
        public string? Message { get; set; }
    }


    public class ResponseResult
    {
        public object? Paging { get; set; }
        public object? Result { get; set; }
    }
}

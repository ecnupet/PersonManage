using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace person.Response
{
    /// <summary>
    /// 响应结果状态
    /// </summary>
    public enum ResponseResultEnum
    {
        [Display(Name = "操作成功")]
        Success,
        [Display(Name = "操作失败")]
        Fail,
        [Display(Name = "已存在")]
        Exist,
        [Display(Name = "未找到")]
        NotFound,
        [Display(Name = "参数非法")]
        Invalid,
        [Display(Name = "参数合法")]
        Valid,
        [Display(Name = "授权通过")]
        Authorized,
        [Display(Name = "授权失败")]
        Unauthorized,
        [Display(Name = "超时失效")]
        TimedOut
    }
    /// <summary>
    /// 响应值模板
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ResponseResultModel<TResult>
    {
        /// <summary>
        /// 响应值状态
        /// </summary>
        public ResponseResultEnum State { get; set; }
        /// <summary>
        /// 细节
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// 细节内容
        /// </summary>
        public TResult Data { get; set; }
        /// <summary>
        /// 生成一个响应值
        /// </summary>
        /// <param name="state"></param>
        /// <param name="data"></param>
        /// <param name="detail"></param>
        public ResponseResultModel(ResponseResultEnum state, TResult data, string detail)
        {
            State = state;
            Data = data;
            Detail = detail;
        }
    }

    public static class ResponseResult
    {
        /// <summary>
        /// 操作成功（带数据内容）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="data"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<TResult> Success<TResult> (TResult data, string detail = null)
        {
            return new ResponseResultModel<TResult>(ResponseResultEnum.Success, data, detail);
        }



        /// <summary>
        /// 操作失败（带数据内容）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="data"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<TResult> Fail<TResult>(TResult data, string detail = null)
        {
            return new ResponseResultModel<TResult>(ResponseResultEnum.Fail, data, detail);
        }



        /// <summary>
        /// 已存在（带数据内容）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="data"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<TResult> Exist<TResult>(TResult data, string detail = null)
        {
            return new ResponseResultModel<TResult>(ResponseResultEnum.Exist, data, detail);
        }



        /// <summary>
        /// 未找到（带数据内容）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="data"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<TResult> NotFound<TResult>(TResult data, string detail = null)
        {
            return new ResponseResultModel<TResult>(ResponseResultEnum.NotFound, data, detail);
        }



        /// <summary>
        /// 参数非法（带数据内容）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="data"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<TResult> Invalid<TResult>(TResult data, string detail = null)
        {
            return new ResponseResultModel<TResult>(ResponseResultEnum.Invalid, data, detail);
        }



        /// <summary>
        /// 参数合法（带数据内容）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="data"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<TResult> Valid<TResult>(TResult data, string detail = null)
        {
            return new ResponseResultModel<TResult>(ResponseResultEnum.Valid, data, detail);
        }



        /// <summary>
        /// 授权通过（带数据内容）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="data"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<TResult> Authorized<TResult>(TResult data, string detail = null)
        {
            return new ResponseResultModel<TResult>(ResponseResultEnum.Authorized, data, detail);
        }



        /// <summary>
        /// 授权失败（带数据内容）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="data"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<TResult> Unauthorized<TResult>(TResult data, string detail = null)
        {
            return new ResponseResultModel<TResult>(ResponseResultEnum.Unauthorized, data, detail);
        }



        /// <summary>
        /// 超时失效（带数据内容）
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="data"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<TResult> TimedOut<TResult>(TResult data, string detail = null)
        {
            return new ResponseResultModel<TResult>(ResponseResultEnum.TimedOut, data, detail);
        }



        /// <summary>
        /// 操作成功（不附带数据内容）
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<Object> Success(string detail = null)
        {
            return new ResponseResultModel<Object>(ResponseResultEnum.Success, null, detail);
        }



        /// <summary>
        /// 操作失败（不附带数据内容）
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<Object> Fail(string detail = null)
        {
            return new ResponseResultModel<Object>(ResponseResultEnum.Fail, null, detail);
        }



        /// <summary>
        /// 已存在（不附带数据内容）
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<Object> Exist(string detail = null)
        {
            return new ResponseResultModel<Object>(ResponseResultEnum.Exist, null, detail);
        }



        /// <summary>
        /// 未找到（不附带数据内容）
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<Object> NotFound(string detail = null)
        {
            return new ResponseResultModel<Object>(ResponseResultEnum.NotFound, null, detail);
        }



        /// <summary>
        /// 参数非法（不附带数据内容）
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<Object> Invalid(string detail = null)
        {
            return new ResponseResultModel<Object>(ResponseResultEnum.Invalid, null, detail);
        }



        /// <summary>
        /// 参数合法（不附带数据内容）
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<Object> Valid(string detail = null)
        {
            return new ResponseResultModel<Object>(ResponseResultEnum.Valid, null, detail);
        }



        /// <summary>
        /// 已授权（不附带数据内容）
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<Object> Authorized(string detail = null)
        {
            return new ResponseResultModel<Object>(ResponseResultEnum.Authorized, null, detail);
        }



        /// <summary>
        /// 未获得授权（不附带数据内容）
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<Object> Unauthorized(string detail = null)
        {
            return new ResponseResultModel<Object>(ResponseResultEnum.Unauthorized, null, detail);
        }



        /// <summary>
        /// 超时（不附带数据内容）
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static ResponseResultModel<Object> TimedOut(string detail = null)
        {
            return new ResponseResultModel<Object>(ResponseResultEnum.TimedOut, null, detail);
        }
    }
}

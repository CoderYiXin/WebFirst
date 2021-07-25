﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace SoEasyPlatform.Code.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileInfoController : BaseController
    {
        public FileInfoController(IMapper mapper) : base(mapper)
        {

        }
        /// <summary>
        /// 获取系统列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getFileInfolist")]
        public ActionResult<ApiResult<TableModel<FileInfoGridViewModel>>> GetFileInfoList([FromForm] FileInfoViewModel model)
        {
            var result = new ApiResult<TableModel<FileInfoGridViewModel>>();
            result.Data = new TableModel<FileInfoGridViewModel>();
            int count = 0;
            var list = Db.Queryable<FileInfo>()
                .WhereIF(!string.IsNullOrEmpty(model.Name), it => it.Name.Contains(model.Name))
                .OrderBy(it=>new { it.Name})
                .Select<FileInfoGridViewModel>()
                .ToPageList(model.PageIndex, model.PageSize, ref count);
            result.Data.Rows = list;
            result.Data.Total = count;
            result.Data.PageSize = model.PageSize;
            result.Data.PageNumber = model.PageIndex;
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [FormValidateFilter]
        [Route("saveFileInfo")]
        public ActionResult<ApiResult<string>> SaveFileInfo([FromForm] FileInfoViewModel model)
        {
            JsonResult errorResult = base.ValidateModel(model.Id);
            if (errorResult != null) return errorResult;
            var saveObject = base.mapper.Map<FileInfo>(model);
            var result = new ApiResult<string>();
            if (saveObject.Id == 0)
            {
                saveObject.IsDeleted = false;
                FileInfoDb.Insert(saveObject);
                result.IsSuccess = true;
                result.Data = Pubconst.MESSAGEADDSUCCESS;
            }
            else
            {
                saveObject.IsDeleted = false;
                FileInfoDb.Update(saveObject);
                result.IsSuccess = true;
                result.Data = Pubconst.MESSAGEADDSUCCESS;
            }
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("deleteFileInfo")]
        public ActionResult<ApiResult<bool>> DeleteFileInfo([FromForm] string model)
        {
            var result = new ApiResult<bool>();
            if (!string.IsNullOrEmpty(model))
            {
                var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DatabaseViewModel>>(model);
                var exp = Expressionable.Create<FileInfo>();
                foreach (var item in list)
                {
                    exp.Or(it => it.Id == item.Id);
                }
                FileInfoDb.Update(it => new FileInfo() { IsDeleted = true }, exp.ToExpression());
            }
            result.IsSuccess = true;
            return result;
        }
    }
}
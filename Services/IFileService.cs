﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Services
{
   public interface IFileService
    {
        Task<byte[]> EncodeFileAsync(IFormFile file);
        Task<byte[]> EncodeFileAsync(string fileName);
        string DecodeImage(byte[] data, string type);

        bool ValidateFileType(IFormFile file);
        bool ValidateFileType(IFormFile file, List<string> fileTypes);

        bool ValidateFileSize(IFormFile file);
        bool ValidateFileSize(IFormFile file, int maxSize);

        string ContentType(IFormFile file);
        int Size(IFormFile file);

    }
}
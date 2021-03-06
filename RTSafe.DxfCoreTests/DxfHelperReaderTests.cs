﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RtSafe.DxfCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtSafe.DxfCore.Tests
{
    [TestClass()]
    public class DxfHelperReaderTests
    {
        [TestMethod()]
        public void ReadTest()
        {
            var reader = new DxfHelperReader();
            using (var ms = File.OpenRead(@"P:\Demo\AutoCAD\WebApplication\Draw2.dxf"))
            {
                reader.Read(ms, 1, "all");              
            }
        }
    }
}
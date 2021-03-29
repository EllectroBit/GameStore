using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using GameStore.Models;
using Microsoft.Data.SqlClient;
using GameStore.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Test
{
    public class AccountUnitTest
    {
        [Fact]
        public void IndexReturnViewResult()
        {
            //Arrange
            var controller = new StoreController(storeContext);
            //Act
            var model = controller.Index();
            //Assert
            Assert.IsType<ViewResult>(model);
        }
    }
}

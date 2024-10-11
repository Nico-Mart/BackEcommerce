﻿using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository 
    {

        public CategoryRepository(NirvanaContext context) : base(context) 
        {   
        }
    }
}

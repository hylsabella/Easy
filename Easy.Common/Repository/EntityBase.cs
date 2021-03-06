﻿using System;

namespace Easy.Common.Repository
{
    public class EntityBase : EntityPrimary
    {
        public string Creater { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public string Editor { get; set; }

        public DateTime? EditDate { get; set; }

        public bool IsDel { get; set; } = false;

        public int Version { get; set; } = 0;
    }
}
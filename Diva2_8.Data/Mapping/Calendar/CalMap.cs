using Diva2.Core.Main.Calendar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Data.Mapping.Calendar
{
    public class CalEventMap : IEntityTypeConfiguration<CalEvent>
    {

        public void Configure(EntityTypeBuilder<CalEvent> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("cal_event");
        }
    }


    public class CalMinutesMap : IEntityTypeConfiguration<CalIniMinute>
    {

        public void Configure(EntityTypeBuilder<CalIniMinute> builder)
        {
            builder.HasKey(x => new { x.PobockaId, x.Minute});
            builder.ToTable("cal_ini_minutes");
            builder.Ignore(d => d.Id);
        }
    }
}

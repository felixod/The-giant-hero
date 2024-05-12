using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SQLBuilder
{
    [PrimaryKey(nameof(Id))]
    public class Department
    {
        /// <summary>
        /// Перечень структурных подраздений предприятия с префиксом
        /// </summary>
        /// <param name="Id">Первичный ключ</param>
        /// <param name="Name">Наименование подразделения</param>
        /// <param name="Prefix">Префикс подразделение</param>
        /// <param name="Code">Код подразделения</param>
        /// <param name="ParentId">Ссылка на родительское подразделение</param>
        /// <param name="Parent">Навигационное свойство для родительского подразделения</param>
        /// <param name="Children">Навигационное свойство для дочерних подразделений</param>
        /// <returns></returns>
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Prefix { get; set; }
        public string? Code { get; set; }
        public int? ParentId { get; set; } // Ссылка на родительское подразделение
        public Department Parent { get; set; } // Навигационное свойство для родительского подразделения
        public ICollection<Department> Children { get; set; } // Навигационное свойство для дочерних подразделений
    }

    /// <summary>
    /// Имена переменных в SCADA
    /// </summary>
    /// <param name="id">Первичный ключ (уникальный номер параметра)</param>
    /// <param name="id_app">Код цеха</param>
    /// <param name="name">Тэг переменной в SCADA (B1A1C10_C, B1A1C7_C и др.)</param>
    /// <param name="type">Тип расчетного выражения (MM, MS и др.)</param>
    /// <param name="Desc">Навигационное свойство со ссылкой на таблицу описаний</param>
    /// <param name="Instr">Навигационное свойство со ссылкой на таблицу выражений SCADA</param>
    /// <param name="Hrs">Навигационное свойство со ссылкой на таблицу значений по часам</param>
    /// <param name="Dat">Навигационное свойство со ссылкой на таблицу значений по 3х-минутам</param>
    /// <returns></returns>

    [PrimaryKey(nameof(id))]
    public class MSPDB_Params
    {
        public int id { get; set; }
        public int id_app { get; set; }
        public required string name { get; set; }
        public required string type { get; set; }

        public MSPDB_Params_Desc? Desc { get; set; }
        public MSPDB_Params_Instr? Instr { get; set; }
        public List<MSPDB_hrs> Hrs { get; set; } = [];
        public List<MSPDB_dat> Dat { get; set; } = [];
    }

    /// <summary>
    /// Описание переменной
    /// </summary>
    /// <param name="Description">Технологическое наименование переменной</param>
    /// <param name="dtb">Дата создания или изменения переменной (Для одного первичного ключа может быть несколько наименований, поэтому выбрать по последней дате)</param>
    /// <param name="id_param">Внешний ключ на таблицу переменных</param>
    /// <param name="Params">Навигационное свойство со ссылкой на таблицу переменных</param>
    /// <returns></returns>
    [PrimaryKey(nameof(dtb), nameof(id_param))]
    public class MSPDB_Params_Desc
    {
        public DateTime dtb { get; set; }
        public int id_param { get; set; }

        public string? Description { get; set; }

        public int? MSPDB_ParamsId { get; set; }
        public MSPDB_Params? Params { get; set; }
    }

    /// <summary>
    ///  Выражение для вычисления переменной в SCADA
    /// </summary>
    /// <param name="Name">Выражение для вычисления</param>
    /// <param name="dtb">Дата создания или изменения переменной (Для одного первичного ключа может быть несколько наименований, поэтому выбрать по последней дате)</param>
    /// <param name="id_param">Внешний ключ на таблицу переменных</param>
    /// <param name="MSPDB_Params">Навигационное свойство со ссылкой на таблицу переменных</param>
    /// <returns></returns>
    [PrimaryKey(nameof(dtb), nameof(id_param))]
    public class MSPDB_Params_Instr
    {
        public DateTime dtb { get; set; }
        public int id_param { get; set; }
        public required string Name { get; set; }

        public int? MSPDB_ParamsId { get; set; }
        public MSPDB_Params? Params { get; set; }
    }

    /// <summary>
    /// Часовые значения на конец часа
    /// </summary>
    /// <param name="dtm">Первичный ключ (Дата\Время)</param>
    /// <param name="Value">Значение на конец часа</param>
    /// <param name="status">Статус значения (0 - достоверное значение, 1 - недостоверное)</param>
    /// <param name="id_param">Внешний ключ на таблицу переменных</param>
    /// <param name="Params">Навигационное свойство со ссылкой на таблицу переменных</param>
    /// <returns></returns>

    [PrimaryKey(nameof(dtm), nameof(id_param))]
    public class MSPDB_hrs
    {
        public DateTime dtm { get; set; }
        public int id_param { get; set; }
        public float Value { get; set; }
        public bool status { get; set; }

        public required MSPDB_Params Params { get; set; }
    }

    /// <summary>
    /// 3-х минутные значения
    /// </summary>
    /// <param name="dtm">Первичный ключ (Метка времени)</param>
    /// <param name="Value">Значение объекта данных</param>
    /// <param name="status">Статус (0 -ОК, 1 - не достоверно, null - отсутствует)</param>
    /// <param name="id_param">Внешний ключ на идентификатор объекта данных</param>
    /// <param name="Params">Навигационное свойство со ссылкой на таблицу переменных</param>
    /// <returns></returns>
    [PrimaryKey(nameof(dtm), nameof(id_param))]
    public class MSPDB_dat
    {
        public DateTime dtm { get; set; }
        public int id_param { get; set; }
        public float Value { get; set; }
        public bool status { get; set; }

        public required MSPDB_Params Params { get; set; }
    }



    public class ApplicationContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<MSPDB_Params> MSPDB_Params { get; set; } = null!;
        public DbSet<MSPDB_dat> MSPDB_dat { get; set; } = null!;
        public DbSet<MSPDB_hrs> MSPDB_hrs { get; set; } = null!;
        public DbSet<MSPDB_Params_Desc> MSPDB_Params_Desc { get; set; } = null!;
        public DbSet<MSPDB_Params_Instr> MSPDB_Params_Instr { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=helloappdb;Trusted_Connection=True;");
            //optionsBuilder.UseSqlServer(@"Server=MSSQL02.TU-UGMK.COM\DB02;Database=helloappdb;Trusted_Connection=True;Integrated Security=true;TrustServerCertificate=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Parent)
                .WithMany(d => d.Children)
                .HasForeignKey(d => d.ParentId);

            modelBuilder.Entity<MSPDB_Params>()
                .HasOne(p => p.Desc)
                .WithOne(n => n.Params)
                .HasForeignKey<MSPDB_Params_Desc>(n => n.id_param);

            modelBuilder.Entity<MSPDB_Params>()
                .HasOne(d => d.Instr)
                .WithOne(p => p.Params)
                .HasForeignKey<MSPDB_Params_Instr>(p => p.id_param);
        }
    }

}

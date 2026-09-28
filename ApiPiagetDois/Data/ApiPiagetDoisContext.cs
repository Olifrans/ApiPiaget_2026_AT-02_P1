using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ApiPiagetDois.Models;

namespace ApiPiagetDois.Data
{
    //public class ApiPiagetDoisContext : DbContext
    //{
    //    public ApiPiagetDoisContext (DbContextOptions<ApiPiagetDoisContext> options)
    //        : base(options)
    //    {
    //    }

    //    public DbSet<ApiPiagetDois.Models.Aluno> Aluno { get; set; } = default!;
    //     public DbSet<ApiPiagetDois.Models.Escola> Escola { get; set; } = default!;
    //    public DbSet<ApiPiagetDois.Models.Professor> Professor { get; set; } = default!;
    //    public DbSet<ApiPiagetDois.Models.AdmEscola> AdmEscola { get; set; } = default!;
    //}



    public class ApiPiagetDoisContext : DbContext
    {
        public ApiPiagetDoisContext(DbContextOptions<ApiPiagetDoisContext> options)
            : base(options)
        {
        }

        public DbSet<Escola> Escolas { get; set; } = default!;
        public DbSet<Professor> Professores { get; set; } = default!;
        public DbSet<Aluno> Alunos { get; set; } = default!;
        public DbSet<AdmEscola> AdmEscolas { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Escola
            modelBuilder.Entity<Escola>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EscolaNome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EscolaEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EscolaTelefone).IsRequired().HasMaxLength(20);

                entity.HasMany(e => e.Professores)
                      .WithOne(p => p.Escola)
                      .HasForeignKey(p => p.EscolaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Alunos)
                      .WithOne(a => a.Escola)
                      .HasForeignKey(a => a.EscolaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.AdmEscolas)
                      .WithOne(a => a.Escola)
                      .HasForeignKey(a => a.EscolaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Professor
            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProfessorNome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ProfessorEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ProfessorTelefone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ProfessorDisciplina).IsRequired().HasMaxLength(100);

                entity.HasMany(p => p.AlunosLecionados)
                      .WithMany(a => a.Professores)
                      .UsingEntity<Dictionary<string, object>>(
                          "AlunoProfessor",
                          j => j.HasOne<Aluno>()
                               .WithMany()
                               .HasForeignKey("AlunoId")
                               .OnDelete(DeleteBehavior.Cascade),
                          j => j.HasOne<Professor>()
                               .WithMany()
                               .HasForeignKey("ProfessorId")
                               .OnDelete(DeleteBehavior.Cascade),
                          j =>
                          {
                              j.HasKey("ProfessorId", "AlunoId");
                              j.ToTable("AlunoProfessor");
                          }
                      );
            });

            // Configure Aluno
            modelBuilder.Entity<Aluno>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AlunoNome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AlunoEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AlunoTelefone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.AlunoMatricula).IsRequired().HasMaxLength(50);

                entity.HasIndex(e => e.AlunoMatricula).IsUnique();
            });

            // Configure AdmEscola
            modelBuilder.Entity<AdmEscola>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AdmNomeFuncionario).HasMaxLength(100);
                entity.Property(e => e.AdmFuncaoFuncionario).HasMaxLength(100);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Base>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.DataAlteracao = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }

}

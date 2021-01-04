﻿// <auto-generated />
using System;
using Equinor.ProCoSys.IPO.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Equinor.ProCoSys.IPO.Infrastructure.Migrations
{
    [DbContext(typeof(IPOContext))]
    partial class IPOContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.HistoryAggregate.History", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ObjectGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ObjectType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Plant")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ObjectGuid")
                        .HasDatabaseName("IX_History_ObjectGuid_ASC");

                    b.ToTable("History");

                    b.HasCheckConstraint("constraint_history_check_valid_event_type", "EventType in ('IpoCompleted','IpoAccepted','IpoSigned','IpoUnaccepted','IpoCreated','IpoEdited','AttachmentUploaded','AttachmentRemoved')");
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Attachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<Guid>("BlobStorageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("InvitationId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedById")
                        .HasColumnType("int");

                    b.Property<string>("Plant")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("InvitationId");

                    b.ToTable("Attachment");
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.CommPkg", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CommPkgNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InvitationId")
                        .HasColumnType("int");

                    b.Property<string>("Plant")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("InvitationId");

                    b.ToTable("CommPkgs");
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CommentText")
                        .IsRequired()
                        .HasMaxLength(4096)
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<int?>("InvitationId")
                        .HasColumnType("int");

                    b.Property<string>("Plant")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("InvitationId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Invitation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(4096)
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndTimeUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MeetingId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedById")
                        .HasColumnType("int");

                    b.Property<Guid>("ObjectGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Plant")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<DateTime>("StartTimeUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.McPkg", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CommPkgNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InvitationId")
                        .HasColumnType("int");

                    b.Property<string>("McPkgNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Plant")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("InvitationId");

                    b.ToTable("McPkgs");
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Participant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("Attended")
                        .HasColumnType("bit");

                    b.Property<Guid?>("AzureOid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FunctionalRoleCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InvitationId")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedById")
                        .HasColumnType("int");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Organization")
                        .HasColumnType("int");

                    b.Property<string>("Plant")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<DateTime?>("SignedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("SignedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortKey")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("InvitationId");

                    b.HasIndex("ModifiedById");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedById")
                        .HasColumnType("int");

                    b.Property<Guid>("Oid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.HistoryAggregate.History", b =>
                {
                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Attachment", b =>
                {
                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Invitation", null)
                        .WithMany("Attachments")
                        .HasForeignKey("InvitationId");
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.CommPkg", b =>
                {
                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Invitation", null)
                        .WithMany("CommPkgs")
                        .HasForeignKey("InvitationId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Comment", b =>
                {
                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Invitation", null)
                        .WithMany("Comments")
                        .HasForeignKey("InvitationId");
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Invitation", b =>
                {
                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.McPkg", b =>
                {
                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Invitation", null)
                        .WithMany("McPkgs")
                        .HasForeignKey("InvitationId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Participant", b =>
                {
                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Invitation", null)
                        .WithMany("Participants")
                        .HasForeignKey("InvitationId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Equinor.ProCoSys.IPO.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("Equinor.ProCoSys.IPO.Domain.AggregateModels.InvitationAggregate.Invitation", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Comments");

                    b.Navigation("CommPkgs");

                    b.Navigation("McPkgs");

                    b.Navigation("Participants");
                });
#pragma warning restore 612, 618
        }
    }
}

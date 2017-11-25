using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PharmaBook.Entities;

namespace PharmaBook.Migrations
{
    [DbContext(typeof(PharmaBookContext))]
    [Migration("20171125061347_modifiedStef-2")]
    partial class modifiedStef2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("PharmaBook.Entities.ChildInvoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<string>("BatchNo");

                    b.Property<string>("Description");

                    b.Property<string>("ExpDt");

                    b.Property<int>("MasterInvID");

                    b.Property<string>("Mfg");

                    b.Property<int>("PrdId");

                    b.Property<int>("Qty");

                    b.HasKey("Id");

                    b.ToTable("InvChild");
                });

            modelBuilder.Entity("PharmaBook.Entities.ChildPO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ProdID");

                    b.Property<int>("Qty");

                    b.Property<string>("Remarks");

                    b.Property<double?>("eachStefPrice");

                    b.Property<int>("masterPOid");

                    b.Property<int?>("stef");

                    b.Property<int?>("tabletsCapsule");

                    b.HasKey("Id");

                    b.ToTable("ChildPO");
                });

            modelBuilder.Entity("PharmaBook.Entities.MasterInvoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DrName");

                    b.Property<DateTime>("InvCrtdate");

                    b.Property<string>("InvId");

                    b.Property<string>("PatientAdres");

                    b.Property<string>("PatientName");

                    b.Property<string>("RegNo");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("InvMaster");
                });

            modelBuilder.Entity("PharmaBook.Entities.MasterPO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("VendorID");

                    b.Property<bool>("isActive");

                    b.Property<DateTime>("placedOrderDt");

                    b.Property<string>("userName");

                    b.HasKey("Id");

                    b.ToTable("MasterPO");
                });

            modelBuilder.Entity("PharmaBook.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MRP");

                    b.Property<string>("batchNo");

                    b.Property<string>("companyName");

                    b.Property<string>("cusUserName");

                    b.Property<double?>("eachStefPrice");

                    b.Property<DateTime>("expDate");

                    b.Property<bool>("isActive");

                    b.Property<string>("lastUpdated");

                    b.Property<string>("name");

                    b.Property<int>("openingStock");

                    b.Property<int?>("stef");

                    b.Property<int?>("tabletsCapsule");

                    b.Property<int?>("vendorID");

                    b.HasKey("Id");

                    b.ToTable("products");
                });

            modelBuilder.Entity("PharmaBook.Entities.PurchasedHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BatchNo");

                    b.Property<string>("ExpDate");

                    b.Property<string>("MRP");

                    b.Property<string>("Mfg");

                    b.Property<string>("Name");

                    b.Property<int>("ProductID");

                    b.Property<string>("Remark");

                    b.Property<string>("cusUserName");

                    b.Property<string>("eachStefPrice");

                    b.Property<DateTime>("purchasedDated");

                    b.Property<string>("qty");

                    b.Property<string>("stef");

                    b.Property<string>("tabletsCapsule");

                    b.Property<int?>("vendorID");

                    b.HasKey("Id");

                    b.ToTable("purchasedHistory");
                });

            modelBuilder.Entity("PharmaBook.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("PharmaBook.Entities.UserProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AccountExpDt");

                    b.Property<string>("Address1");

                    b.Property<string>("Address2");

                    b.Property<DateTime>("CreatedDt");

                    b.Property<string>("DLNo");

                    b.Property<string>("Email");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Mobile");

                    b.Property<string>("Name");

                    b.Property<DateTime>("lastLogin");

                    b.Property<string>("subTitle");

                    b.Property<string>("userName");

                    b.HasKey("Id");

                    b.ToTable("UserProfile");
                });

            modelBuilder.Entity("PharmaBook.Entities.Vendor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("cusUserName");

                    b.Property<string>("vendorAddress");

                    b.Property<string>("vendorCompnay");

                    b.Property<string>("vendorMobile");

                    b.Property<string>("vendorName");

                    b.HasKey("Id");

                    b.ToTable("vendors");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("PharmaBook.Entities.User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("PharmaBook.Entities.User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PharmaBook.Entities.User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}

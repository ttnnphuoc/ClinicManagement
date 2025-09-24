using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStaffRoleToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Map existing string values to enum integer values
            // SuperAdmin=0, ClinicManager=1, Doctor=2, Nurse=3, Receptionist=4, Accountant=5, Pharmacist=6
            migrationBuilder.Sql(@"
                ALTER TABLE ""Staff"" 
                ALTER COLUMN ""Role"" TYPE integer 
                USING CASE ""Role""
                    WHEN 'SuperAdmin' THEN 0
                    WHEN 'ClinicManager' THEN 1
                    WHEN 'Doctor' THEN 2
                    WHEN 'Nurse' THEN 3
                    WHEN 'Receptionist' THEN 4
                    WHEN 'Accountant' THEN 5
                    WHEN 'Pharmacist' THEN 6
                    ELSE 4
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Staff",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}

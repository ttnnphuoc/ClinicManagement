using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTreatmentHistoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId",
                table: "TreatmentHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodPressure",
                table: "TreatmentHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChiefComplaint",
                table: "TreatmentHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClinicId",
                table: "TreatmentHistories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "DifferentialDiagnosis",
                table: "TreatmentHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FollowUpInstructions",
                table: "TreatmentHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeartRate",
                table: "TreatmentHistories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "TreatmentHistories",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextAppointmentDate",
                table: "TreatmentHistories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhysicalExamination",
                table: "TreatmentHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RespiratoryRate",
                table: "TreatmentHistories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StaffId",
                table: "TreatmentHistories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Symptoms",
                table: "TreatmentHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature",
                table: "TreatmentHistories",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TreatmentPlan",
                table: "TreatmentHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "TreatmentHistories",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentHistories_AppointmentId",
                table: "TreatmentHistories",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentHistories_ClinicId",
                table: "TreatmentHistories",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentHistories_StaffId",
                table: "TreatmentHistories",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentHistories_Appointments_AppointmentId",
                table: "TreatmentHistories",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentHistories_Clinics_ClinicId",
                table: "TreatmentHistories",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentHistories_Staff_StaffId",
                table: "TreatmentHistories",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentHistories_Appointments_AppointmentId",
                table: "TreatmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentHistories_Clinics_ClinicId",
                table: "TreatmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentHistories_Staff_StaffId",
                table: "TreatmentHistories");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentHistories_AppointmentId",
                table: "TreatmentHistories");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentHistories_ClinicId",
                table: "TreatmentHistories");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentHistories_StaffId",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "BloodPressure",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "ChiefComplaint",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "DifferentialDiagnosis",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "FollowUpInstructions",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "HeartRate",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "NextAppointmentDate",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "PhysicalExamination",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "RespiratoryRate",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "Symptoms",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "TreatmentPlan",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "TreatmentHistories");
        }
    }
}

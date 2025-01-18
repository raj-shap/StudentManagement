using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement.Migrations
{
    /// <inheritdoc />
    public partial class StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedure = @"
                CREATE PROCEDURE GetStudentSubjectMarks
                AS
                BEGIN
                    SELECT
                        s.Name,
                        s.Id AS StudentId,
                        subj.Id AS SubjectId,
                        subj.SubjectName,
                        m.Mark
                    FROM Marks m
                    INNER JOIN Students s ON m.StudentId = s.Id
                    INNER JOIN Subjects subj ON m.SubjectId = subj.Id
                    WHERE m.IsDeleted = 0 AND s.IsDeleted = 0 AND subj.IsDeleted = 0
                END
                ";
            migrationBuilder.Sql(procedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedure = @"DROP PROCEDURE GetStudentSubjectMarks";
            migrationBuilder.Sql(procedure);
        }
    }
}

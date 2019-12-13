using Xunit;
using FluentAssertions;
using System;

namespace Courses.Domain.Tests {
    public class CourseSummaryTests {
        [Fact]
        public void EmptySummary_Test() {
            var summary = new CourseSummary();

            summary.AgeMin.Should().BeLessThan(0);
            summary.AgeMax.Should().BeLessThan(0);
            summary.AgeSum.Should().Be(0);
            summary.AgeAvg.Should().Be(0);
            summary.StudentCount.Should().Be(0);
        }

        [Fact]
        public void AddStudents_Test() {
            var summary = new CourseSummary();

            summary.AddStudent(30);

            summary.AgeMin.Should().Be(30);
            summary.AgeMax.Should().Be(30);
            summary.AgeSum.Should().Be(30);
            summary.AgeAvg.Should().Be(30);
            summary.StudentCount.Should().Be(1);

            summary.AddStudent(20);

            summary.AgeMin.Should().Be(20);
            summary.AgeMax.Should().Be(30);
            summary.AgeSum.Should().Be(50);
            summary.AgeAvg.Should().Be(25);
            summary.StudentCount.Should().Be(2);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddStudentNegativeAge_Test(int age) {
            var summary = new CourseSummary();

            summary.Invoking(it => it.AddStudent(age))
                .Should().Throw<ArgumentException>();
        }
    }
}
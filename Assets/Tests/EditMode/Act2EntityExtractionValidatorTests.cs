using Ghost.Puzzles.EntityExtraction;
using NUnit.Framework;

namespace Ghost.Tests.EditMode
{
    public sealed class Act2EntityExtractionValidatorTests
    {
        [Test]
        public void Validate_WhenSubmittedSpansExactlyMatch_ReturnsCorrect()
        {
            var roomType = CreateRoomType();
            var timeType = CreateTimeType();
            var expected = new[]
            {
                new EntitySpan(6, 3, roomType),
                new EntitySpan(13, 3, timeType)
            };
            var submitted = new[]
            {
                new EntitySpan(13, 3, new EntityType("time", EntityCategory.System)),
                new EntitySpan(6, 3, new EntityType("room", EntityCategory.Custom))
            };

            var result = EntityExtractionValidator.Validate(expected, submitted);

            Assert.That(result.IsCorrect, Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void Validate_WhenExpectedSpanIsMissing_ReturnsIncorrect()
        {
            var expected = new[]
            {
                new EntitySpan(6, 3, CreateRoomType()),
                new EntitySpan(13, 3, CreateTimeType())
            };
            var submitted = new[]
            {
                new EntitySpan(6, 3, CreateRoomType())
            };

            var result = EntityExtractionValidator.Validate(expected, submitted);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("Missing span"));
        }

        [Test]
        public void Validate_WhenBoundaryMatchesButTypeIsWrong_ReturnsIncorrect()
        {
            var expected = new[]
            {
                new EntitySpan(6, 3, CreateRoomType())
            };
            var submitted = new[]
            {
                new EntitySpan(6, 3, CreateObjectType())
            };

            var result = EntityExtractionValidator.Validate(expected, submitted);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("Wrong type"));
        }

        [Test]
        public void Validate_WhenTypeMatchesButBoundaryIsWrong_ReturnsIncorrect()
        {
            var expected = new[]
            {
                new EntitySpan(6, 3, CreateRoomType())
            };
            var submitted = new[]
            {
                new EntitySpan(7, 3, CreateRoomType())
            };

            var result = EntityExtractionValidator.Validate(expected, submitted);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("Wrong boundary"));
        }

        [Test]
        public void Validate_WhenSubmittedSpanIsExtra_ReturnsIncorrect()
        {
            var expected = new[]
            {
                new EntitySpan(6, 3, CreateRoomType())
            };
            var submitted = new[]
            {
                new EntitySpan(6, 3, CreateRoomType()),
                new EntitySpan(20, 7, CreateObjectType())
            };

            var result = EntityExtractionValidator.Validate(expected, submitted);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("Extra span"));
        }

        [Test]
        public void Validate_WhenSubmittedSpanIsDuplicated_ReturnsIncorrect()
        {
            var expected = new[]
            {
                new EntitySpan(6, 3, CreateRoomType())
            };
            var submitted = new[]
            {
                new EntitySpan(6, 3, CreateRoomType()),
                new EntitySpan(6, 3, CreateRoomType())
            };

            var result = EntityExtractionValidator.Validate(expected, submitted);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.Errors, Has.Some.Contains("Duplicate submitted span"));
        }

        private static EntityType CreateRoomType()
        {
            return new EntityType("room", EntityCategory.Custom);
        }

        private static EntityType CreateObjectType()
        {
            return new EntityType("object", EntityCategory.Custom);
        }

        private static EntityType CreateTimeType()
        {
            return new EntityType("time", EntityCategory.System);
        }
    }
}

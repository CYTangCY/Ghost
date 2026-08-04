using Ghost.Presentation.Act6BackendResponse;
using Ghost.Puzzles.BackendResponse;
using NUnit.Framework;
using System.Reflection;

namespace Ghost.Tests.EditMode
{
    public sealed class Act6BackendResponseValidatorTests
    {
        [Test]
        public void ReferenceBackendActionAndResponsePass()
        {
            var result = Act6BackendResponseValidator.Validate(
                Act6BackendResponseData.LabRecordsId,
                Act6BackendResponseData.FetchLabClosingTimeId,
                Act6BackendResponseData.LabHoursResponseId);

            Assert.That(result.IsCorrect, Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public void EmptyBoardFailsAtDataSource()
        {
            var result = Act6BackendResponseValidator.Validate(null, null, null);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.FirstBrokenRoleId, Is.EqualTo(Act6BackendResponseData.DataSourceRoleId));
            Assert.That(result.Errors.Count, Is.EqualTo(3));
        }

        [Test]
        public void WrongSourceStopsAtDataSource()
        {
            var result = Act6BackendResponseValidator.Validate(
                Act6BackendResponseData.RoomDirectoryId,
                Act6BackendResponseData.FetchLabClosingTimeId,
                Act6BackendResponseData.LabHoursResponseId);

            Assert.That(result.FirstBrokenRoleId, Is.EqualTo(Act6BackendResponseData.DataSourceRoleId));
            Assert.That(result.Errors[0], Does.Contain("closing time"));
        }

        [Test]
        public void WrongActionStopsAtAction()
        {
            var result = Act6BackendResponseValidator.Validate(
                Act6BackendResponseData.LabRecordsId,
                Act6BackendResponseData.FindObjectRoomId,
                Act6BackendResponseData.LabHoursResponseId);

            Assert.That(result.FirstBrokenRoleId, Is.EqualTo(Act6BackendResponseData.ActionRoleId));
            Assert.That(result.Errors[0], Does.Contain("room"));
        }

        [Test]
        public void WrongResponseStopsAtResponse()
        {
            var result = Act6BackendResponseValidator.Validate(
                Act6BackendResponseData.LabRecordsId,
                Act6BackendResponseData.FetchLabClosingTimeId,
                Act6BackendResponseData.ObjectRoomResponseId);

            Assert.That(result.FirstBrokenRoleId, Is.EqualTo(Act6BackendResponseData.ResponseRoleId));
            Assert.That(result.Errors[0], Does.Contain("does not answer"));
        }

        [Test]
        public void ValidationResultTracksEachRoleFromValidator()
        {
            var result = Act6BackendResponseValidator.Validate(
                Act6BackendResponseData.RoomDirectoryId,
                Act6BackendResponseData.FetchLabClosingTimeId,
                Act6BackendResponseData.LabHoursResponseId);

            Assert.That(result.IsRoleCorrect(Act6BackendResponseData.DataSourceRoleId), Is.False);
            Assert.That(result.IsRoleCorrect(Act6BackendResponseData.ActionRoleId), Is.True);
            Assert.That(result.IsRoleCorrect(Act6BackendResponseData.ResponseRoleId), Is.True);
        }

        [Test]
        public void ReturningFilledRoleRestoresPaletteAndClearsStaleValidation()
        {
            var controller = new Act6BackendInteractionController();
            controller.BeginAfterOnboarding();
            controller.PlaceCardOnRole(
                Act6BackendResponseData.LabRecordsId,
                Act6BackendResponseData.DataSourceRoleId);
            var validation = Act6BackendResponseValidator.Validate(
                Act6BackendResponseData.LabRecordsId,
                Act6BackendResponseData.FetchLabClosingTimeId,
                Act6BackendResponseData.LabHoursResponseId);
            var lastValidationProperty = typeof(Act6BackendInteractionController).GetProperty(
                nameof(Act6BackendInteractionController.LastValidation),
                BindingFlags.Instance | BindingFlags.Public);
            var lastValidationSetter = lastValidationProperty?.GetSetMethod(true);
            Assert.That(lastValidationSetter, Is.Not.Null);
            lastValidationSetter.Invoke(controller, new object[] { validation });

            Assert.That(controller.LastValidation, Is.Not.Null);
            Assert.That(controller.ReturnRoleCardToPalette(
                Act6BackendResponseData.DataSourceRoleId), Is.True);
            Assert.That(controller.GetPlacedCardId(
                Act6BackendResponseData.DataSourceRoleId), Is.Empty);
            Assert.That(controller.IsCardPlaced(Act6BackendResponseData.LabRecordsId), Is.False);
            Assert.That(controller.LastValidation, Is.Null);
        }

        [Test]
        public void CardFromWrongRoleCannotSatisfySocket()
        {
            var result = Act6BackendResponseValidator.Validate(
                Act6BackendResponseData.FetchLabClosingTimeId,
                Act6BackendResponseData.LabRecordsId,
                Act6BackendResponseData.LabHoursResponseId);

            Assert.That(result.IsCorrect, Is.False);
            Assert.That(result.FirstBrokenRoleId, Is.EqualTo(Act6BackendResponseData.DataSourceRoleId));
            Assert.That(result.Errors[0], Does.Contain("belongs to ACTION"));
        }
    }
}

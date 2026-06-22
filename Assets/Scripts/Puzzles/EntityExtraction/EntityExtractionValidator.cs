using System;
using System.Collections.Generic;

namespace Ghost.Puzzles.EntityExtraction
{
    public static class EntityExtractionValidator
    {
        public static EntityExtractionResult Validate(
            IEnumerable<EntitySpan> expected,
            IEnumerable<EntitySpan> submitted)
        {
            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            if (submitted == null)
            {
                throw new ArgumentNullException(nameof(submitted));
            }

            var errors = new List<string>();
            var expectedSpans = CreateSpanList(expected, "Expected", errors);
            var submittedSpans = CreateSpanList(submitted, "Submitted", errors);
            var uniqueSubmittedSpans = CreateUniqueSubmittedSpans(submittedSpans, errors);
            var unmatchedExpectedSpans = CreateUnmatchedExpectedSpans(expectedSpans, uniqueSubmittedSpans);

            ValidateExpectedSpans(unmatchedExpectedSpans, uniqueSubmittedSpans, errors);
            ValidateSubmittedSpans(expectedSpans, unmatchedExpectedSpans, uniqueSubmittedSpans, errors);

            return new EntityExtractionResult(errors.Count == 0, errors);
        }

        private static List<EntitySpan> CreateSpanList(
            IEnumerable<EntitySpan> source,
            string listName,
            ICollection<string> errors)
        {
            var spans = new List<EntitySpan>();

            foreach (var span in source)
            {
                if (span == null)
                {
                    errors.Add($"{listName} span list contains a null span.");
                    continue;
                }

                spans.Add(span);
            }

            return spans;
        }

        private static List<EntitySpan> CreateUniqueSubmittedSpans(
            IEnumerable<EntitySpan> submittedSpans,
            ICollection<string> errors)
        {
            var uniqueSubmittedSpans = new List<EntitySpan>();

            foreach (var submittedSpan in submittedSpans)
            {
                if (ContainsSpan(uniqueSubmittedSpans, submittedSpan))
                {
                    errors.Add($"Duplicate submitted span {FormatSpan(submittedSpan)} was submitted more than once.");
                    continue;
                }

                uniqueSubmittedSpans.Add(submittedSpan);
            }

            return uniqueSubmittedSpans;
        }

        private static List<EntitySpan> CreateUnmatchedExpectedSpans(
            IEnumerable<EntitySpan> expectedSpans,
            IReadOnlyList<EntitySpan> uniqueSubmittedSpans)
        {
            var unmatchedExpectedSpans = new List<EntitySpan>();

            foreach (var expectedSpan in expectedSpans)
            {
                if (!ContainsSpan(uniqueSubmittedSpans, expectedSpan))
                {
                    unmatchedExpectedSpans.Add(expectedSpan);
                }
            }

            return unmatchedExpectedSpans;
        }

        private static void ValidateExpectedSpans(
            IEnumerable<EntitySpan> unmatchedExpectedSpans,
            IReadOnlyList<EntitySpan> uniqueSubmittedSpans,
            ICollection<string> errors)
        {
            foreach (var expectedSpan in unmatchedExpectedSpans)
            {
                var wrongTypeSpan = FindSpanWithSameBoundary(uniqueSubmittedSpans, expectedSpan);
                if (wrongTypeSpan != null)
                {
                    errors.Add(
                        $"Wrong type for span {FormatBoundary(expectedSpan)}: expected {FormatType(expectedSpan.Type)} but submitted {FormatType(wrongTypeSpan.Type)}.");
                    continue;
                }

                var wrongBoundarySpan = FindSpanWithSameType(uniqueSubmittedSpans, expectedSpan);
                if (wrongBoundarySpan != null)
                {
                    errors.Add(
                        $"Wrong boundary for {FormatType(expectedSpan.Type)}: expected {FormatBoundary(expectedSpan)} but submitted {FormatBoundary(wrongBoundarySpan)}.");
                    continue;
                }

                errors.Add($"Missing span {FormatSpan(expectedSpan)}.");
            }
        }

        private static void ValidateSubmittedSpans(
            IReadOnlyList<EntitySpan> expectedSpans,
            IReadOnlyList<EntitySpan> unmatchedExpectedSpans,
            IEnumerable<EntitySpan> uniqueSubmittedSpans,
            ICollection<string> errors)
        {
            foreach (var submittedSpan in uniqueSubmittedSpans)
            {
                if (ContainsSpan(expectedSpans, submittedSpan))
                {
                    continue;
                }

                if (FindSpanWithSameBoundary(unmatchedExpectedSpans, submittedSpan) != null)
                {
                    continue;
                }

                if (FindSpanWithSameType(unmatchedExpectedSpans, submittedSpan) != null)
                {
                    continue;
                }

                errors.Add($"Extra span {FormatSpan(submittedSpan)} was submitted.");
            }
        }

        private static bool ContainsSpan(IEnumerable<EntitySpan> spans, EntitySpan target)
        {
            foreach (var span in spans)
            {
                if (span == target)
                {
                    return true;
                }
            }

            return false;
        }

        private static EntitySpan FindSpanWithSameBoundary(IEnumerable<EntitySpan> spans, EntitySpan target)
        {
            foreach (var span in spans)
            {
                if (span.Start == target.Start && span.Length == target.Length)
                {
                    return span;
                }
            }

            return null;
        }

        private static EntitySpan FindSpanWithSameType(IEnumerable<EntitySpan> spans, EntitySpan target)
        {
            foreach (var span in spans)
            {
                if (span.Type == target.Type)
                {
                    return span;
                }
            }

            return null;
        }

        private static string FormatSpan(EntitySpan span)
        {
            return $"{FormatBoundary(span)} as {FormatType(span.Type)}";
        }

        private static string FormatBoundary(EntitySpan span)
        {
            return $"start {span.Start}, length {span.Length}";
        }

        private static string FormatType(EntityType type)
        {
            return $"'{type.Id}' ({type.Category})";
        }
    }

    public sealed class EntityExtractionResult
    {
        private readonly string[] errors;

        internal EntityExtractionResult(bool isCorrect, IEnumerable<string> errors)
        {
            IsCorrect = isCorrect;
            this.errors = errors == null ? Array.Empty<string>() : new List<string>(errors).ToArray();
        }

        public bool IsCorrect { get; }

        public IReadOnlyList<string> Errors => errors;
    }
}

  // Focus search input on page load
        $(document).ready(function () {
            $('#searchTerm').focus();
        });

        // ESC key clears search box
        $(document).on('keydown', function (e) {
            if (e.key === "Escape") {
                $('#searchTerm').val('');
                $('#searchForm').submit();
            }
        });
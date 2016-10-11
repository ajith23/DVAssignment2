﻿$(document).ready(function () {
    Modernizr.load({
        test: Modernizr.svg,
        yep: 'js/matchTree.js',
        nope: ['js/jquery-1.9.1.min.js', 'js/fallback.js']
    });

    $('#yearSelector button').click(function () {
        $(this).addClass('active').siblings().removeClass('active');

        $('#roundTreesvg').html('');
        $('#result').html('');
        $('#matchDataresult').html('');
        $('#aceChart').html('');
        $('#totalPointChart').html('');
        $('#winnerChart').html('');
        $('#errorChart').html('');
        $('#matchData').html('');
        loadRoundTree($(this).html());
    });
});
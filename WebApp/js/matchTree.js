var radius = 300, numRounds = 7, segmentWidth = radius / (numRounds + 1);

var partition = d3.layout.partition()
  .sort(null)
  .size([2 * Math.PI, radius]) // x maps to angle, y to radius
  .value(function (d) { return 1; }); //Important!

var arc = d3.svg.arc()
  .startAngle(function (d) { return d.x; })
  .endAngle(function (d) { return d.x + d.dx; })
  .innerRadius(function (d) { return d.y; })
  .outerRadius(function (d) { return d.y + d.dy; });

function translateSVG(x, y) {
    return 'translate(' + x + ',' + y + ')';
}

function rotateSVG(a, x, y) {
    a = a * 180 / Math.PI;
    return 'rotate(' + a + ')';
    return 'rotate(' + a + ',' + x + ',' + y + ')';
}

function arcSVG(mx0, my0, rx, ry, xrot, larc, sweep, mx1, my1) {
    return 'M' + mx0 + ',' + my0 + ' A' + rx + ',' + ry + ' ' + xrot + ' ' + larc + ',' + sweep + ' ' + mx1 + ',' + my1;
}

var label = function (d) {
    if (d.x === 0 && d.y === 0)
        return '';
    var t = rotateSVG(d.x + 0.5 * d.dx - Math.PI * 0.5, 0, 0);
    t += translateSVG(d.y + 0.5 * d.dy, 0);
    t += d.x >= Math.PI ? rotateSVG(Math.PI) : '';
    return t;
}

function surname(d) {
    return d.name.split(' ')[0];
}

function fullname(d) {
    //var s = d.name.split(' ');
    //return s.length === 3 ? s[2] + ' ' + s[0] + ' ' + s[1] : s[1] + ' ' + s[0];
    return d.name;
}

function result(d) {
    var m = d.match;
    var res = '';
    if (m !== undefined) {
        for (var i = 1; i <= 5; i++) {
            if (m['w' + i] !== 0 && m['l' + i] !== 0)
                res += m['w' + i] + '-' + m['l' + i] + ' ';
        }
    }
    return res;
}

function playerHover(d) {
    var c = surname(d);
    d3.selectAll('g#player-labels text')
      .style('fill', 'white');

    // Highlight this player + children
    d3.select('g#player-labels text.' + c + '.round-' + d.round)
      .style('fill', 'yellow');

    if (d.round != 1) {
        c = surname(d.children[0]);
        d3.select('g#player-labels text.' + c + '.round-' + +(d.round - 1))
          .style('fill', 'yellow');

        c = surname(d.children[1]);
        d3.select('g#player-labels text.' + c + '.round-' + +(d.round - 1))
          .style('fill', 'yellow');
    }

    d3.select('#result').text(fullname(d.children[0]) + ' beat ' + fullname(d.children[1]));
        
}

function playerClick(d) {
    var c = surname(d);
    d3.selectAll('g#player-labels text')
      .style('fill', 'white');

    // Highlight this player + children
    d3.select('g#player-labels text.' + c + '.round-' + d.round)
      .style('fill', 'red');

    if (d.round != 1) {
        c = surname(d.children[0]);
        d3.select('g#player-labels text.' + c + '.round-' + +(d.round - 1))
          .style('fill', 'red');

        c = surname(d.children[1]);
        d3.select('g#player-labels text.' + c + '.round-' + +(d.round - 1))
          .style('fill', 'red');
    }

    if (d.matchD !== undefined) {
        var xAxisArray = JSON.parse(d.matchD).XAxis;
        var winnerArray = JSON.parse(d.matchD).WinnerData;
        var loserArray = JSON.parse(d.matchD).LoserData;
        for (var i = 0; i < xAxisArray.length; i++) {
            xAxisArray[i] = xAxisArray[i].replace(/([A-Z])/g, ' $1').trim()
        }
        xAxisArray.splice(0, 0, "xA");
        winnerArray.splice(0, 0, fullname(d.children[0]));
        loserArray.splice(0, 0, fullname(d.children[1]));
        var chartData = [
                xAxisArray,
                winnerArray,
                loserArray
        ];
        /*var chartData = [
                ['xA', 'test', 'test2', 'test3'],
                [fullname(d.children[0]), 30, 200, 100],
                [fullname(d.children[1]), 130, 100, 140]
        ];*/
        loadMatchData(chartData, fullname(d.children[0]), fullname(d.children[1]));
    }
}

function loadMatchData(matchData, winner, loser) {
    var xData = ['xA', 'test', 'test2', 'test3'];
    var chart1 = c3.generate({
        bindto: '#matchData',
        data: {
            x: 'xA',
            columns: matchData,
            type: 'bar',
            onclick: function (d, i) {
                console.log("onclick", d, i);
            },
            //onmouseover: function (d, i) { console.log("onmouseover", d, i); },
            //onmouseout: function (d, i) { console.log("onmouseout", d, i); }
        },
        bar:{
            width: {
                ratio:0.5
            }
        },
        legend: {
            position: 'top'
        },
        axis: { x: { type: 'category', show: true } },
        labels: true,
        tooltip: {
            show: true
        },
        color: {
            pattern: ["#5F3A81", "#56AEC7", "#F09348", "#A196A9"]
        },
        grid: {
            x: {
                show: true
            },
            y: {
                show: true,
                lines: []
            }
        }
    });

    /*d3.select("#matchData svg").append("text")
    .attr("x", 150)
    .attr("y", 10)
    .style("text-anchor", "middle")
    .text(winner + ' beats ' + loser);*/
    d3.select('#matchDataresult').text(winner + ' beats ' + loser);
}

var xCenter = radius, yCenter = radius;


var currentYearFile = 'data/' + $('#yearSelector').find('.active').html() + '.json';

loadRoundTree(2003);

function loadRoundTree(year)
{
    d3.select('#roundTreesvg').transition();
    var svg = d3.select('#roundTreesvg').append('g').attr('transform', translateSVG(xCenter, yCenter));
    var currentYearFile = 'data/' + year + '.json';
    d3.json(currentYearFile, function (err, root) {
        // console.log(root);
        var chart = svg.append('g');
        chart.datum(root).selectAll('g')
          .data(partition.nodes)
          .enter()
          .append('g');

        // We use three groups: segments, round labels & player labels. This is to achieve a layering effect.
        // Segments
        chart.selectAll('g')
          .append('path')
          .attr('d', arc)
          .on('click', playerClick)
          .on('mouseover', playerHover);

        // Round labels
        var rounds = ['Round 1', 'Round 2', 'Round 3', 'Round 4', 'Quarter finals', 'Semi finals', 'Final'];
        var roundLabels = svg.append('g').attr('id', 'round-labels');
        roundLabels.selectAll('path')
          .data(rounds)
          .enter()
          .append('path')
          .attr('d', function (d, i) {
              var offset = (numRounds - i + 0.5) * segmentWidth - 10;
              return arcSVG(-offset, 0, offset, offset, 0, 1, 1, offset, 0);
          })
          .style({ 'fill': 'none', 'stroke': 'none' })
          .attr('id', function (d, i) { return 'round-label-' + +(i + 1); });

        roundLabels.selectAll('text')
          .data(rounds)
          .enter()
          .append('text')
          .append('textPath')
          .attr('xlink:href', function (d, i) { return '#round-label-' + +(i + 1); })
          .attr('startOffset', '50%')
          .text(function (d) { return d; });

        // Player labels
        var playerLabels = svg.append('g').attr('id', 'player-labels');
        playerLabels.datum(root).selectAll('g')
          .data(partition.nodes)
          .enter()
          .append('text')
          .text(function (d, i) { return i === 0 ? surname(d) : d.name.slice(0, 3); })
          .attr('transform', label)
          .attr('dy', '0.4em')
          .attr('class', function (d) { return surname(d) + ' round-' + +(d.round); });

    });
}
function element(id) {
  return document.getElementById(id)
}

function createGraph() {
    GDATA.sort((a, b) => {
        return a.year - b.year;
    })

    let totals = [], screens = [], provides = [];
    let total = 0, screen = 0, provide = 0;
    let year = null;

    GDATA.forEach(e => {
        if (e.year != year) {
            if (year != null) {
                totals.push(total);
                screens.push(screen);
                provides.push(provide);
            }

            year = e.year;
            total = parseInt(e.total);
            screen = parseInt(e.screen);
            provide = parseInt(e.provide);
        } else {
            total += parseInt(e.total);
            screen += parseInt(e.screen);
            provide += parseInt(e.provide);
        }
    });

    totals.push(total);
    screens.push(screen);
    provides.push(provide);

    Highcharts.chart('graphContainer', {
        title: {
            text: 'แนวโน้มจำนวนแต่ละประเภท ตั้งแต่ปี 2561-2563'
        },
        yAxis: {
            title: {
                text: 'จำนวน (คน)'
            }
        },
        xAxis: {
            title: {
                text: 'ปี'
            },
            tickInterval: 1
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle'
        },
        plotOptions: {
            series: {
                label: {
                    connectorAllowed: false
                },
                pointStart: 2561
            }
        },
        series: [{
            name: 'ทั้งหมด',
            data: totals
        }, {
            name: 'ทำการคัดกรอง',
            data: screens
        }, {
            name: 'รับการจัดสรร',
            data: provides
        }],
        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    legend: {
                        layout: 'horizontal',
                        align: 'center',
                        verticalAlign: 'bottom'
                    }
                }
            }]
        }
    });
}


function createBar() {
    GDATA.sort((a, b) => {
        return a.year - b.year;
    })

    let totals = [], screens = [], provides = [];
    let total = 0, screen = 0, provide = 0;
    let year = null;

    GDATA.forEach(e => {
        if (e.year != year) {
            if (year != null) {
                totals.push(total);
                screens.push(screen);
                provides.push(provide);
            }

            year = e.year;
            total = parseInt(e.total);
            screen = parseInt(e.screen);
            provide = parseInt(e.provide);
        } else {
            total += parseInt(e.total);
            screen += parseInt(e.screen);
            provide += parseInt(e.provide);
        }
    });

    totals.push(total);
    screens.push(screen);
    provides.push(provide);

    Highcharts.chart('barContainer', {
        chart: {
            type: 'bar'
        },
        title: {
            text: 'เปรียบเทียบจำนวนแต่ละประเภทตามปี'
        },
        xAxis: {
            categories: ['ทั้งหมด', 'ทำการคัดกรอง', 'รับการจัดสรร'],
            title: {
                text: null
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: 'จำนวน (คน)',
                align: 'high'
            },
            labels: {
                overflow: 'justify'
            }
        },
        plotOptions: {
            bar: {
                dataLabels: {
                    enabled: true
                }
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'top',
            x: -40,
            y: 80,
            floating: true,
            borderWidth: 1,
            backgroundColor:
                Highcharts.defaultOptions.legend.backgroundColor || '#FFFFFF',
            shadow: true
        },
        credits: {
            enabled: false
        },
        series: [{
            name: 'ปี 2561',
            //data: [totals[0], screens[0], provides[0]]
            data: [totals[0], screens[0], provides[0]]
        }, {
            name: 'ปี 2562',
            data: [totals[1], screens[1], provides[1]]
        }, {
            name: 'ปี 2563',
            data: [totals[2], screens[2], provides[2]]
        },]
    });
}


function createPie(screen, provide) {
    Highcharts.chart('pieContainer', {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: 'สัดส่วนจำนวนที่ทำคัดกรอง'
        },
        accessibility: {
            point: {
                valueSuffix: '%'
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %'
                }
            }
        },
        series: [{
            name: 'Brands',
            colorByPoint: true,
            data: [{
                name: 'ไม่ได้รับจัดสรร',
                y: screen - provide,
                sliced: true,
                selected: true
            }, {
                name: 'รับการจัดสรร',
                y: provide
            }]
        }]
    });
}


function createTable(d, a_id) {
    let thead = element('thead');
    let tbody = element('tbody');
    let tmpD = d;

    function createCol(txt, notheader) {
        let c = document.createElement(notheader == undefined ? 'th' : 'td');
        c.textContent = txt;
        return c
    }

    function createHeader(a_id) {
        console.log(a_id)
        thead.appendChild(createCol(a_id == undefined ? 'เขต' : 'หน่วยงาน'));
        thead.appendChild(createCol('จำนวนทั้งหมด'));
        thead.appendChild(createCol('จำนวนคัดกรอง'));
        thead.appendChild(createCol('จำนวนที่ได้รับจัดสรร'));
    }

    function addBody(d, gotEvent) {
        let tr = document.createElement('tr');

        tr.appendChild(createCol(d.name, true))
        tr.appendChild(createCol(d.total, true))
        tr.appendChild(createCol(d.screen, true))
        tr.appendChild(createCol(d.provide, true))

        tr.addEventListener('click', e => {
            createTable(tmpD, d.a_id)
        });

        tbody.appendChild(tr);
    }

    function createBody(data, aid) {
        let a_id = null, tmp = {};

        if (aid == undefined) {
            data.sort((a, b) => { return a.a_id - b.a_id })

            data.forEach(e => {
                if (e.a_id != a_id) {
                    if (a_id != null) addBody(tmp);
                    a_id = e.a_id;
                    tmp = { a_id: e.a_id, name: e.area, total: parseInt(e.total), screen: parseInt(e.screen), provide: parseInt(e.provide) }
                } else {
                    tmp.total += parseInt(e.total);
                    tmp.screen += parseInt(e.screen);
                    tmp.provide += parseInt(e.provide);
                }
            })

            addBody(tmp, true);
        } else {
            data.filter(e => { return e.a_id == aid })
            data.sort((a, b) => { return a.o_id - b.o_id })

            data.forEach(e => {
                addBody({ a_id: e.a_id, name: e.o_name, total: parseInt(e.total), screen: parseInt(e.screen), provide: parseInt(e.provide) });
            });
        }
    }

    thead.innerHTML = "";
    tbody.innerHTML = "";

    if (a_id == undefined) {
        createHeader();
        createBody(d);
    } else {
        createHeader(a_id);
        createBody(d, a_id);
    }
}


function bindYear() {
    let tmp = GDATA.filter(e => {
        return e.year == element('yearDrpDwn').value
    })

    let sumTotal = 0, sumScreen = 0, sumProvide = 0;

    tmp.forEach(e => {
        sumTotal += parseInt(e.total);
        sumScreen += parseInt(e.screen);
        sumProvide += parseInt(e.provide);
    })

    totalH.textContent = sumTotal;
    screenH.textContent = sumScreen;
    provideH.textContent = sumProvide;

    createPie(sumScreen, sumProvide);
    createTable(tmp);
}

var GDATA;
$(document).ready(function () {
    element('yearDrpDwn').addEventListener('click', e => {
        bindYear();
    });

    $.ajax({
        type: 'GET',
        url: '/dbservice.svc/getscreendata',
        dataType: 'json',
        success: (data) => {
            GDATA = data;

            bindYear();
            createGraph();
            createBar();
        },
    });
});


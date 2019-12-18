function drawCirclePacking(hierarchyBy){
    d3.selectAll("g > *").remove()
var svg = d3.select("svg"),
    width = +svg.attr("width"),
    height = +svg.attr("height");
    var ranges;
    $.ajax ({ url: "ScalesRanges.json", method: "GET", async: false})
    .success(function (response) {
        ranges = response;
    });

var pack = d3.pack()
    .size([width - 2, height - 2])
    .padding(3)

d3.json('/OutputsFiles/FinalStatisticsOutput.json').then(function (dataset) {
    var root = d3.hierarchy(dataset)
        .sum(function (d) { return d['code']; })
		.sort(function(a, b) { return b['code'] - a['code'] || b['code'] - a['code']; });
    pack(root);
    var node = svg.select("g")
        .selectAll("g")
        .data(root.descendants())
        .enter().append("g")
        .text(function(d) { return d.name })
        .attr("transform", function (d) { return "translate(" + d.x + "," + d.y + ")"; })
        .attr("class", function (d) { return "node" + (!d.children ? " node--leaf" : d.data.name); })
        .each(function (d) { d.node = this; })
        .on("mouseover", hovered(true))
        .on("mouseout", hovered(false));

       
    node.append("circle")
        .attr("id", function (d) { return "node-" + d.id; })
        .attr("r", function (d) { return d.r; })
        .style("fill", function (d) { 
            if(d.height === 0){
                return drawLeaf(d.data[hierarchyBy], ranges[hierarchyBy]);
            }
            else{
                return drawCirclesPacks(d.depth, 0, root.height - 1);
            }
         });
    node.append("svg:text")
    .text(function(d) { return  d.data.typename; })
    .attr('text-anchor', "middle")
    .attr('dx', function(d) { !d.children ? 0 : d.r })
    .attr('dy', function(d) { !d.children ? 0 : d.r })
    .attr('textLength', function(d) { return !d.children ? d.r * 2 : d.r; })
	
}) 
function hovered(hover) {
    return function(d) {
      d3.selectAll(d.ancestors().map(function(d) { return d.node; })).classed("node--hover", hover);
    };
  }
}

function doStatisticsBy(hierarchyBy){
	drawCirclePacking(hierarchyBy);
}

function drawLeaf(value, ranges){
    var color = d3.scaleLinear()
    .domain(ranges.range)
    .range(ranges.colors);
    return color(value);
}

function drawCirclesPacks(value, minHierarchyValue, maxHierarchyValue){
    var color = d3.scaleLinear()
    .domain([minHierarchyValue, maxHierarchyValue])
    .range(['#ecfcff', '#5edfff']);
    return color(value);
}


$(document).ready(function(){
var btnContainer = document.getElementById("buttons");

var btns = btnContainer.getElementsByClassName("decidingButton");

for (var i = 0; i < btns.length; i++) {
  btns[i].addEventListener("click", function() {
    var current = document.getElementsByClassName("active");
    current[0].className = current[0].className.replace(" active", "");
    this.className += " active";
  });
}
    });



function go (value){
    try{
        drawCirclePacking(value)
    }catch(error){
        console.log(error);
    }
}
    
function drawCirclePacking(hierarchyBy){
var svg = d3.select("svg"),
    width = +svg.attr("width"),
    height = +svg.attr("height");



var pack = d3.pack()
    .size([width - 2, height - 2])
    .padding(3)

d3.json('/OutputsFiles/FinalStatisticsOutput.json').then(function (dataset) {
    console.log(dataset);
    var root = d3.hierarchy(dataset)
        .sum(function (d) { return d[hierarchyBy]; })
		.sort(function(a, b) { return b[hierarchyBy] - a[hierarchyBy] || b[hierarchyBy] - a[hierarchyBy]; });
    pack(root);
    var node = svg.select("g")
        .selectAll("g")
        .data(root.descendants())
        .enter().append("g")
        .attr("transform", function (d) { return "translate(" + d.x + "," + d.y + ")"; })
        .attr("class", function (d) { return "node" + (!d.children ? " node--leaf" : d.depth ? "" : " node--root"); })
        .each(function (d) { d.node = this; })
        .on("mouseover", hovered(true))
        .on("mouseout", hovered(false));
    node.append("circle")
        .attr("id", function (d) { return "node-" + d.id; })
        .attr("r", function (d) { return d.r; })
        .style("fill", function (d) { 
            if(d.height === 0){
                return decideAboutColor(d.data.hierarchyBy, true);
            }
            else{
                return decideAboutColor(d.depth, false, );
            }
         });
}) 
function hovered(hover) {
    return function(d) {
      d3.selectAll(d.ancestors().map(function(d) { return d.node; })).classed("node--hover", hover);
    };
  }
}

function doStatisticsByTotalCommitsNumber(){
	let hierarchyBy = "allcommitsnumber";
	drawCirclePacking(hierarchyBy);
	disableOneDecidingButton("totalCommitsButton");
}

function disableOneDecidingButton(buttonId){
	$( document ).ready(function() {
		$(".decidingButton").attr("disabled", false);
		$("#".concat(buttonId)).attr("disabled", true);
	});
}

function decideAboutColor(value, isLeaf, minHierarchyValue, maxHierarchyValue){
    if(isLeaf === true){
        var color = d3.scaleLinear()
    .domain([minHierarchyValue, maxHierarchyValue])
    .range(['#ffd369', '#940a37']);
    return color(value);
    }
    else{
        var color = d3.scaleLinear()
    .domain([minHierarchyValue, maxHierarchyValue])
    .range(['#ecfcff', '#5edfff']);
    return color(value);
    }


    
}
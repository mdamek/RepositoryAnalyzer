var svg = d3.select("svg"),
    width = +svg.attr("width"),
    height = +svg.attr("height");

var color = d3.scaleLinear()
    .domain([0, 3])
    .range(['grey', 'red']);

var pack = d3.pack()
    .size([width - 2, height - 2])
    .padding(3)

d3.json("occupation.json").then(function (dataset) {
    var root = d3.hierarchy(dataset)
        .sum(function (d) { return d.size; })
        .sort(function (a, b) { return b.size - a.size || b.size - a.size; });
    pack(root);
    console.log(root);
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
        .style("fill", function (d) { return color(d.depth); });
}) 
function hovered(hover) {
    return function(d) {
      d3.selectAll(d.ancestors().map(function(d) { return d.node; })).classed("node--hover", hover);
    };
  }
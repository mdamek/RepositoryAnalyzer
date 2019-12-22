var actualHierarchy;
var node;

function drawCirclePacking(hierarchyBy) {
  actualHierarchy = hierarchyBy;
  d3.selectAll("g > *").remove();
  var svg = d3.select("#mainChart"),
    width = +svg.attr("width"),
    height = +svg.attr("height");
  var ranges;
  $.ajax({
    url: "CompartmentValues.json",
    method: "GET",
    async: false
  }).success(function(response) {
    ranges = response;
  });

  var pack = d3
    .pack()
    .size([width - 2, height - 2])
    .padding(3);

  d3.json("/OutputsFiles/FinalStatisticsOutput.json").then(function(dataset) {
    var root = d3
      .hierarchy(dataset)
      .sum(function(d) {
        return d["code"];
      })
      .sort(function(a, b) {
        return b["code"] - a["code"] || b["code"] - a["code"];
      });
    pack(root);
    node = svg
      .select("g")
      .selectAll("g")
      .data(root.descendants())
      .enter()
      .append("g")
      .text(function(d) {
        return d.name;
      })
      .attr("transform", function(d) {
        return "translate(" + d.x + "," + d.y + ")";
      })
      .attr("class", function(d) {
        return "node" + (!d.children ? " node--leaf" : d.data.name);
      })
      .each(function(d) {
        d.node = this;
      })
      .on("mouseover", hovered(true))
      .attr("id", function(d) {
        if (d.height === 0) return d.data.id;
        else return -1;
      })
      .on("mouseout", hovered(false));

    node
      .append("circle")
      .attr("id", function(d) {
        return "node-" + d.id;
      })
      .attr("r", function(d) {
        return d.r;
      })
      .style("fill", function(d) {
        if (d.height === 0) {
          return drawLeaf(d.data[hierarchyBy], ranges[hierarchyBy]);
        } else {
          return drawCirclesPacks(d.depth, 0, root.height - 1);
        }
      });
  });
  function hovered(hover) {
    return function(d) {
      if (hover) {
        $("#actualValues").css("visibility", "visible");
        $("#actualSelectedName").text(" Name: " + d.data.name);
        if (d.height === 0) {
          $("#actualSelectedId").text(" Id: " + d.data.id);
          $("#actualSelectedValue").text(
            " " + actualHierarchy + ": " + d.data[actualHierarchy]
          );
        }
      } else {
        $("#actualValues").css("visibility", "hidden");
      }
      d3.selectAll(
        d.ancestors().map(function(d) {
          return d.node;
        })
      ).classed("node--hover", hover);
    };
  }
}

function doStatisticsBy(hierarchyBy) {
  drawCirclePacking(hierarchyBy);
}

function drawLeaf(value, ranges) {
  var color = d3
    .scaleLinear()
    .domain(ranges.range)
    .range(ranges.colors);
  return color(value);
}

function drawCirclesPacks(value, minHierarchyValue, maxHierarchyValue) {
  var color = d3
    .scaleLinear()
    .domain([minHierarchyValue, maxHierarchyValue])
    .range(["#ecfcff", "#5edfff"]);
  return color(value);
}

$(document).ready(function() {
  var rawData;
  var globalValues;
  $.ajax({
    url: "MetricsRawToArray.json",
    method: "GET",
    async: false
  }).success(function(response) {
    rawData = response;
  });

  $.ajax({
    url: "GlobalValues.json",
    method: "GET",
    async: false
  }).success(function(response) {
    globalValues = response;
  });

  $("#rawDataTable").DataTable({
    data: rawData,
    pageLength: 25,
    columns: [
      {
        title: "Id",
        data: "Id"
      },
      {
        title: "Name",
        data: "TypeName"
      },
      {
        title: "LOC",
        data: "Code"
      },
      {
        title: "Comments",
        data: "Comment"
      },
      {
        title: "Commits",
        data: "AllCommitsNumber"
      },
      {
        title: "MI",
        data: "MaintainabilityIndex"
      },
      {
        title: "Cyclo",
        data: "CyclomaticComplexity"
      },
      {
        title: "CC",
        data: "ClassCoupling"
      },
      {
        title: "DiT",
        data: "DepthOfInheritance"
      },
      {
        title: "All",
        data: "BadQualityMetricsNumber"
      }
    ]
  });

  $("#globalDataTable").DataTable({
    data: globalValues,
    "searching": false,
    "paging": false,
    "ordering": false,
    "info": false,
    columns: [
      {
      title: "Value",
      data: "name"
      },
      {
        title: "LOC",
        data: "LOC"
      },
      {
        title: "Comments",
        data: "Comments"
      },
      {
        title: "Commits",
        data: "Commits"
      },
      {
        title: "MI",
        data: "MI"
      },
      {
        title: "Cyclo",
        data: "Cyclo"
      },
      {
        title: "CC",
        data: "CC"
      },
      {
        title: "DiT",
        data: "DiT"
      },
      {
        title: "All",
        data: "All"
      }
    ]
  });

  $("#rawDataTable").on("mouseover", "tbody tr", function() {
    var idToShow = $(this)[0].cells[0].textContent;
    $("#" + idToShow)[0].classList.add("node--hover--big");
  });

  $("#rawDataTable").on("mouseout", "tbody tr", function() {
    var idToRemove = $(this)[0].cells[0].textContent;
    $("#" + idToRemove)[0].classList.remove("node--hover--big");
  });

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

function go(value) {
  try {
    drawCirclePacking(value);
  } catch (error) {
    console.log(error);
  }
}

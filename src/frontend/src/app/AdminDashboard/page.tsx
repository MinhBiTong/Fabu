"use client"

import {    BarChart,Bar,Cell,PieChart,Pie,LineChart,Line, XAxis, YAxis,Tooltip,CartesianGrid,  ResponsiveContainer} from "recharts";


export default function AdminDashboard() {
 const data = [
  { name: "Jan", value: 1200 },
  { name: "Feb", value: 2100 },
  { name: "Mar", value: 800 },
  { name: "Apr", value: 1600 },
  { name: "May", value: 900 },
  { name: "Jun", value: 1700 },
  { name: "Jul", value: 1400 },
  { name: "Aug", value: 2200 },
  { name: "Sep", value: 1800 },
  { name: "Oct", value: 2000 },
  { name: "Nov", value: 1400 },
  { name: "Dec",  }
];

 const Countrydata = [
    { name: "Vietnam", value: 4005 },
    { name: "America", value: 12000},
    { name: "China", value: 2240 },
    { name: "Arab", value: 13232 }
  ];


  const Circledata = [
    { name: "Dataplans", value: 1200 },
    { name: "Recharges", value: 600 },
    { name: "Products", value: 2240 }
  ];
  const Circlecolor = ["#4f46e5", "#22c55e", "#f59e0b"];

  const Circletotal = Circledata.reduce((sum, item) => sum + item.value, 0);

  return ( 

  <>
  <div className="DashboardContainer"> 
    <div className="DashboardSidebar">
       <div className="DashboardChoices">Finance</div>
       <div className="DashboardChoices">Idontfuckinknow</div>
       <div className="DashboardChoices">Salesofsomething</div>
        <div className="DashboardChoices">Idontfuckinknow</div>
       <div className="DashboardChoices">Salesofsomething</div>
    </div>


  <div className="DashboardMainContent">
     <div className="FinanceLine1">
        <div className="GridFinance">

            <div className="GridLine1">
                 <div className="Gridbox"></div>
                     <div className="Gridbox"></div>
            </div>

            <div className="GridLine2">
                    <div className="Gridbox"></div>
                        <div className="Gridbox"></div>
            </div>

        </div>
        <div className="MonthFinance">
          <div className="Charter"> 
    <ResponsiveContainer>
              <LineChart data={data}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis tickFormatter={(value) => `$${value}`}/>
                <Tooltip formatter={(value) => `$${value}`} />
                <Line
                  type="monotone"
                  dataKey="value"
                  stroke="#0c00f5"
                  strokeWidth={2}
                  dot={false}
                />
              </LineChart>
            </ResponsiveContainer>

          </div>
        </div>
        </div>

     <div className="FinanceLine2">
      <div className="CircleChart">
        <div className="ChartContainer">
      <ResponsiveContainer width="100%" height="60%">
          <PieChart>
            <Pie
              data={Circledata}
              dataKey="value"
              nameKey="name"
              cx="50%"
              cy="50%"
              outerRadius={90}
              label={({ value }) =>
                `${((value / Circletotal) * 100).toFixed(0)}%`
              }
            >
              {Circledata.map((entry, index) => (
                <Cell
                  key={index}
                  fill={Circlecolor[index % Circlecolor.length]}
                />
              ))}
            </Pie>

            <Tooltip formatter={(value) => `$${value}`} />
          </PieChart>
        </ResponsiveContainer>
       <div className="PieLegend">
        {Circledata.map((item, index) => (
          <div className="LegendItem" key={index}>
          <span
         className="LegendColor"
        style={{ backgroundColor: Circlecolor[index % Circlecolor.length] }}
            ></span>

            <span className="LegendText">
              {item.name} (
              {((item.value / Circletotal) * 100).toFixed(0)}%)
            </span>
          </div>
        ))}
      </div>


        </div>

      </div>
       <div className="CountryBarChart">
            <div className="CountryContainer">
              <ResponsiveContainer width="100%" height="100%">
        <BarChart data={Countrydata}>
          <CartesianGrid strokeDasharray="3 3" />

          <XAxis dataKey="name" />
          <YAxis />

          <Tooltip formatter={(value) => `$${value}`} />

          <Bar
            dataKey="value"
            fill="#2823c4"
          />
        </BarChart>
      </ResponsiveContainer>



        </div>
      </div>


     </div>



     </div>



  </div>

  
  
  </>
  )
}
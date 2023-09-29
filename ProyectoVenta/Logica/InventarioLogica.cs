using ProyectoVenta.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoVenta.Logica
{
    public class InventarioLogica
    {
        private static InventarioLogica _instancia = null;

        public InventarioLogica()
        {

        }

        public static InventarioLogica Instancia
        {

            get
            {
                if (_instancia == null) _instancia = new InventarioLogica();
                return _instancia;
            }
        }



        public List<Inventario> Resumen(string fechainicio = "", string fechafin = "")
        {
            List<Inventario> oLista = new List<Inventario>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT");
                    query.AppendLine("prod.Codigo,");
                    query.AppendLine("prod.Descripcion,");
                    query.AppendLine("prod.Categoria,");
                    query.AppendLine("prod.Almacen,");
                    query.AppendLine("ISNULL(ent.Entradas, 0) AS Entradas,");
                    query.AppendLine("ISNULL(sal.Salidas, 0) AS Salidas,");
                    query.AppendLine("prod.Stock,");
                    query.AppendLine("(SELECT ISNULL(FORMAT(sal.TotalIngresos, 'N2'), '0.00')) AS TotalEgresos,");
                    query.AppendLine("(SELECT ISNULL(FORMAT(sal.TotalIngresos, 'N2'), '0.00')) AS TotalIngresos");
                    query.AppendLine("FROM");
                    query.AppendLine("(");
                    query.AppendLine("SELECT DISTINCT");
                    query.AppendLine("CAST(IdProducto AS VARCHAR(50)) AS IdProducto,");
                    query.AppendLine("CAST(Codigo AS VARCHAR(50)) AS Codigo,");
                    query.AppendLine("CAST(Descripcion AS VARCHAR(MAX)) AS Descripcion,");
                    query.AppendLine("CAST(Categoria AS VARCHAR(50)) AS Categoria,");
                    query.AppendLine("CAST(Almacen AS VARCHAR(50)) AS Almacen,");
                    query.AppendLine("CAST(Stock AS VARCHAR(50)) AS Stock");
                    query.AppendLine("FROM");
                    query.AppendLine("(");
                    query.AppendLine("SELECT");
                    query.AppendLine("p.IdProducto,");
                    query.AppendLine("p.Codigo,");
                    query.AppendLine("p.Descripcion,");
                    query.AppendLine("p.Categoria,");
                    query.AppendLine("p.Almacen,");
                    query.AppendLine("p.Stock");
                    query.AppendLine("FROM");
                    query.AppendLine("DETALLE_ENTRADA de");
                    query.AppendLine("INNER JOIN");
                    query.AppendLine("ENTRADA e ON e.IdEntrada = de.IdEntrada");
                    query.AppendLine("INNER JOIN");
                    query.AppendLine("PRODUCTO p ON p.IdProducto = de.IdProducto");
                    query.AppendLine("WHERE");
                    query.AppendLine("CAST(CAST(FechaRegistro AS varchar(20)) AS DATETIME) BETWEEN CAST(@pfechainicio1 AS DATETIME) AND CAST(@pfechafin1 AS DATETIME)");
                    query.AppendLine("UNION ALL");
                    query.AppendLine("SELECT");
                    query.AppendLine("p.IdProducto,");
                    query.AppendLine("p.Codigo,");
                    query.AppendLine("p.Descripcion,");
                    query.AppendLine("p.Categoria,");
                    query.AppendLine("p.Almacen,");
                    query.AppendLine("p.Stock");
                    query.AppendLine("FROM");
                    query.AppendLine("DETALLE_SALIDA ds");
                    query.AppendLine("INNER JOIN");
                    query.AppendLine("SALIDA s ON s.IdSalida = ds.IdSalida");
                    query.AppendLine("INNER JOIN");
                    query.AppendLine("PRODUCTO p ON p.IdProducto = ds.IdProducto");
                    query.AppendLine("WHERE");
                    query.AppendLine("CAST(CAST(FechaRegistro AS varchar(20)) AS DATE) BETWEEN CAST(@pfechainicio2 AS DATETIME) AND CAST(@pfechafin2 AS DATETIME)");
                    query.AppendLine(") temp");
                    query.AppendLine(") prod");
                    query.AppendLine("LEFT JOIN");
                    query.AppendLine("(");
                    query.AppendLine("SELECT");
                    query.AppendLine("p.IdProducto,");
                    query.AppendLine("SUM(de.Cantidad) AS Entradas,");
                    query.AppendLine("SUM(CAST(CAST(de.SubTotal AS VARCHAR(20)) AS NUMERIC)) AS TotalEgresos");
                    query.AppendLine("FROM");
                    query.AppendLine("PRODUCTO p");
                    query.AppendLine("INNER JOIN");
                    query.AppendLine("DETALLE_ENTRADA de ON de.IdProducto = p.IdProducto");
                    query.AppendLine("INNER JOIN");
                    query.AppendLine("ENTRADA e ON e.IdEntrada = de.IdEntrada");
                    query.AppendLine("WHERE");
                    query.AppendLine("CAST(CAST(FechaRegistro AS VARCHAR(20)) AS DATE) BETWEEN CAST(@pfechainicio3 AS DATETIME) AND CAST(@pfechafin3 AS DATETIME)");
                    query.AppendLine("GROUP BY");
                    query.AppendLine("p.IdProducto,");
                    query.AppendLine("CAST(p.Descripcion AS VARCHAR(MAX)),");
                    query.AppendLine("CAST(p.Categoria AS VARCHAR(MAX)),");
                    query.AppendLine("CAST(p.Almacen AS VARCHAR(MAX))");
                    query.AppendLine(") ent ON ent.IdProducto = prod.IdProducto");
                    query.AppendLine("LEFT JOIN");
                    query.AppendLine("(");
                    query.AppendLine("SELECT");
                    query.AppendLine("p.IdProducto,");
                    query.AppendLine("SUM(ds.Cantidad) AS Salidas,");
                    query.AppendLine("SUM(CAST(CAST(ds.SubTotal AS VARCHAR(20)) AS NUMERIC)) AS TotalIngresos");
                    query.AppendLine("FROM");
                    query.AppendLine("PRODUCTO p");
                    query.AppendLine("INNER JOIN");
                    query.AppendLine("DETALLE_SALIDA ds ON ds.IdProducto = p.IdProducto");
                    query.AppendLine("INNER JOIN");
                    query.AppendLine("SALIDA s ON s.IdSalida = ds.IdSalida");
                    query.AppendLine("WHERE");
                    query.AppendLine("CAST(CAST(FechaRegistro AS varchar(20)) AS DATE) BETWEEN CAST(@pfechainicio4 AS DATETIME) AND CAST(@pfechafin4 AS DATETIME)");
                    query.AppendLine("GROUP BY");
                    query.AppendLine("p.IdProducto");
                    query.AppendLine(") sal ON sal.IdProducto = prod.IdProducto;");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.Add(new SqlParameter("@pfechainicio1", fechainicio));
                    cmd.Parameters.Add(new SqlParameter("@pfechafin1", fechafin));
                    cmd.Parameters.Add(new SqlParameter("@pfechainicio2", fechainicio));
                    cmd.Parameters.Add(new SqlParameter("@pfechafin2", fechafin));
                    cmd.Parameters.Add(new SqlParameter("@pfechainicio3", fechainicio));
                    cmd.Parameters.Add(new SqlParameter("@pfechafin3", fechafin));
                    cmd.Parameters.Add(new SqlParameter("@pfechainicio4", fechainicio));
                    cmd.Parameters.Add(new SqlParameter("@pfechafin4", fechafin));
                    cmd.CommandType = System.Data.CommandType.Text;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            oLista.Add(new Inventario()
                            {
                                Codigo = dr["Codigo"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Categoria = dr["Categoria"].ToString(),
                                Almacen = dr["Almacen"].ToString(),
                                Entradas = dr["Entradas"].ToString(),
                                Salidas = dr["Salidas"].ToString(),
                                Stock = dr["Stock"].ToString(),
                                TotalEgresos = dr["TotalEgresos"].ToString(),
                                TotalIngresos = dr["TotalIngresos"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oLista = new List<Inventario>();
            }
            return oLista;
        }


    }
}

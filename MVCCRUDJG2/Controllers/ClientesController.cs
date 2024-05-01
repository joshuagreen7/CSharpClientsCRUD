using Microsoft.AspNetCore.Mvc;
using MVCCRUDJG2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace MVCCRUDJG2.Controllers
{
    public class ClientesController : Controller
    {

        public readonly IConfiguration _config;

        public ClientesController(IConfiguration IConfig)
        {
            _config = IConfig;

        }   //fin del constructor....
        public IActionResult Index()
        {
            return View();
        } //fin del index

        //-----------------------------------------LISTAR clientes----------------------------------------------------
        IEnumerable<Clientes> clientes()
        {
            List<Clientes> cli = new List<Clientes>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("usp_list_clients", cn);
                //aperturamos la base de datos.
                cn.Open();
                //realizamos la respectiva ejecucion
                SqlDataReader dr = cmd.ExecuteReader();
                //aplicamos un bucle while...
                while (dr.Read())
                {
                    cli.Add(new Clientes()
                    {
                        //recuperamos lo que viene de la base de datos..
                        //y almacenamos en las propiedades....
                        Idcliente = dr.GetInt32(0),
                        nombre = dr.GetString(1),
                        apellido = dr.GetString(2),
                        dni = dr.GetString(3),
                        tipoCliente = dr.GetInt32(4)
                    });  //fin de agregar listado Clientes...                       
                }   //fin del bucle while..

            }   //fin del using.....

            //retornamos el listado
            return cli;

        }   //fin de ienumerable de listado de Clientes...

        //retorna la lista de todos los Clientes registrados en la BD...
        public async Task<IActionResult> ListadoClientes()
        {
            //retornamos hacia la vista...
            return View(await Task.Run(() => clientes()));
        }   //fin del metodo listado Clientes...

        //--------------codigo para REGISTRAR un nuevo cliente----------------------

        //listado para cargar el select de tiposclientes...
        //codigo  para recuperar registros de tiposclientes...
        IEnumerable<TipoCliente> tipos()

        {
            List<TipoCliente> tip = new List<TipoCliente>();
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("usp_list_client_types", cn);
                //aperturamos la base de datos...
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                //aplicamos un while...
                while (dr.Read())
                {
                    //recupermos los datos de la base de datos
                    //y almacenamos en las propiedades...
                    tip.Add(new TipoCliente()
                    {
                        Idtipocliente = dr.GetInt32(0),
                        nombtipocli = dr.GetString(1)

                    });   //fin de agregar al listado

                }  //fin del bucle while...
            }   //fin de using...

            //retornamos la data recuperada
            return tip;
        }   //fin del Ienumerable tipos...

        [HttpGet]

        //sirve para cargar el select de datos Idtipocliente,nombtipocli....
        public async Task<IActionResult> Create()
        {
            ViewBag.tipos = new SelectList(await Task.Run(() => tipos()), "Idtipocliente", "nombtipocli");
            Clientes cliente = new Clientes();
            //retornamos los valores
            return View(cliente);

        }  //fin del metodo...

        [HttpPost]
        public async Task<IActionResult> Create(Clientes model)
        {
            //aplicamos un if....

            if (!ModelState.IsValid)
            {
                ViewBag.tipos = new SelectList(await Task.Run(() => tipos()), "Idtipocliente", "nombtipocli", model.tipoCliente);
                return View(model);
            }    //fin del if...

            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                //aplicamos un try catch..
                try
                {
                    //aperturamos la base de datos...
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_insert_client", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //agregamos los parametros...
                    cmd.Parameters.AddWithValue("@clinom", model.nombre);
                    cmd.Parameters.AddWithValue("@cliape", model.apellido);
                    cmd.Parameters.AddWithValue("@clidni", model.dni);
                    cmd.Parameters.AddWithValue("@tipocliid", model.tipoCliente);
                    //realizamos la ejecucion...
                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"registro insertado {c} de cliente";
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }   //fin del catch....

            }   //fin del using....
            //enviamos el mensaje a la vista...
            ViewBag.mensaje = mensaje;
            ViewBag.tipos = new SelectList(await Task.Run(() => tipos()), "Idtipocliente", "nombtipocli", model.tipoCliente);
            //redireccionamos
            return RedirectToAction("ListadoClientes", "Clientes");

        }  //fin del metodo create POST...
        //----------------------------------EDITAR-------------------------------------------------------------
        Clientes Buscar(int id)
        {
            Clientes? reg = clientes().Where(v => v.Idcliente == id).FirstOrDefault();
            //retornamos el valor buscado
            return reg;
        }    //fin del metodo buscar...
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Clientes reg = Buscar(id);
            //aplicamos una condicion...
            if (reg == null) return RedirectToAction("ListadoClientes");
            ViewBag.tipos = new SelectList(await Task.Run(() => tipos()), "Idtipocliente", "nombtipocli", reg.tipoCliente);
            return View(reg);

        }    //fin del metodo edit GET...
        [HttpPost]
        public async Task<IActionResult> Edit(Clientes model)
        {
            //aplicamos una condicion...
            if (!ModelState.IsValid)
            {
                ViewBag.tipos = new SelectList(await Task.Run(() => tipos()), "Idtipocliente", "nombtipocli", model.tipoCliente);
                //retornamos el valor
                return View(model);

            }    //fin de la condicion...
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                //aplicamos el try catch...
                try
                {
                    //aperturamos la base de datos...
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_update_cliente",cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cliid", model.Idcliente);
                    cmd.Parameters.AddWithValue("@clinom", model.nombre);
                    cmd.Parameters.AddWithValue("@cliape", model.apellido);
                    cmd.Parameters.AddWithValue("@clidni", model.dni);
                    cmd.Parameters.AddWithValue("@tipocliid", model.tipoCliente);
           
                    //realizamos la respectiva ejecucion...
                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"registro actualizado {c} cliente";

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }   //fin del try catch...

            }    //fin del using...
            //enviamos a la vista...
            ViewBag.mensaje = mensaje;
            ViewBag.tipos = new SelectList(await Task.Run(() => tipos()), "Idtipocliente", "nombtipocli", model.tipoCliente);
            //redireccionamos
            return RedirectToAction("ListadoClientes", "Clientes");

        }   //fin del metodo edit POST..

        //------------------- Método para ELIMINAR CLIENTES-----------------------------------------------------
        [HttpGet]
         public async Task<IActionResult> Delete(int id)
        {
            Clientes reg = Buscar(id);
            //aplicamos un if
            if (reg == null) return RedirectToAction("listadoClientes");
            ViewBag.tipos = new SelectList(await Task.Run(() => tipos()), "idtipocliente", "nombtipocli", reg.tipoCliente);
            //retornamos a la vista.
            return View(reg);

        }   //fin del metodo delete GET....

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteCliente(int id)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                try
                {
                    //abrimos conexion...
                    cn.Open();
                    //aperturamos la base de datos...
                    SqlCommand cmd = new SqlCommand("usp_delete_clients", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //agregamos el parametro...
                    cmd.Parameters.AddWithValue("@cliid", id);
                    //realizamos la ejecucion...
                    int c = cmd.ExecuteNonQuery();
                    mensaje = $"registro eliminado {c} cliente";

                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }

            }    //fin del metodo using....liberamos la conexion...
            //enviamos el mensaje hacia la vista...
            ViewBag.mensaje = mensaje;
            //redireccionamos hacia el listado de clientes...
            return RedirectToAction("ListadoClientes");
        }   //fin del metodo delete POST...

    } //Fin de class cliente controller.....
} //fin de namespac....
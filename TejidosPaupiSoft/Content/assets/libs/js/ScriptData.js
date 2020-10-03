

var CalculadoraInput = document.getElementById("Calculadora")
var Precio = document.getElementById("PrecioInput")
var Cantidad = document.getElementById("CantidadInput")


CalculadoraInput.addEventListener("change", (e) => {
    Precio.value = Calcular(e.target.value, Cantidad.value)
    console.log(e.target.value)
})

Cantidad.addEventListener("change", (e) => {
    if (CalculadoraInput.value != "" && CalculadoraInput.value != 0) {
    Precio.value = Calcular(CalculadoraInput.value, Cantidad.value)
        
    }
})


function Calcular(CalculadoraValor, CantidadValor) {

    let resultado = parseInt(CalculadoraValor * (CantidadValor / 100))

    return resultado

}
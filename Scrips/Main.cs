using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Main : MonoBehaviour {
	public Boton der_bot, izq_bot;
	public GameObject[] flechas;
	public float muestra, espera;
	public int ran;
	public bool apreta, over, presed;
	void Start () {
		espera = 0.25f;
		StartCoroutine(lazo());

	}
	IEnumerator lazo(){
		ran = Random.Range(0,3);if(ran == 2) ran = Random.Range(0,2);//borrar
		apreta = true;
		flechas[ran].SetActive(true);
		yield return new WaitForSecondsRealtime(muestra);
		if(presed){
			flechas[ran].SetActive(false);
			apreta = false;
			presed = false;
		}else{
			over = true;
		}


		yield return new WaitForSecondsRealtime(espera);
		StartCoroutine(lazo());
	}
	void ejecuta(bool isder){
		if(apreta){
			switch(ran){
			case 0://se deveria apretar derecha
				if(isder){
					presed = true;
				}else{
					over = true;
				}
				break;
			case 1://se deveria apretar izquierda
				if(!isder){
					presed = true;
				}else{
					over = true;
				}
				break;
			case 2://se deveria apretar los dos
				
				break;
			}
		}else{
			over = true;
		}
	}

	void Update () {
		if(der_bot.down){
			ejecuta(true);
			Debug.Log("Se apreto el boton derecho");
		}
		if(izq_bot.down){
			ejecuta(false);
			Debug.Log("Se apreto el boton izquierdo");
		}
		if(over){//se perdio la partida
			Debug.Log("fin de la partida");
			Debug.Break();
		}
	}
}
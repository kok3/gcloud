package com.ztgame.audio.ui;

//import xmu.swordbearer.audio.AudioWrapper;
import com.ztgame.audio.AudioManager;
import com.ztgame.audio.NetConfig;
import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

public class MainActivity extends Activity {
	String LOG = "MainActivity";

	//private AudioWrapper audioWrapper;

	private AudioManager audioManager;

	// View
	private Button btnStartRecord;
	private Button btnStopRecord;
	private Button btnStartListen;
	private Button btnStopListen;
	private Button btnExit;
	private EditText ipEditText;

	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.main);
		//audioWrapper = AudioWrapper.getInstance();

		audioManager = AudioManager.getInstance();

		initView();
	}

	private void initView() {
		btnStartRecord = (Button) findViewById(R.id.startRecord);
		btnStartListen = (Button) findViewById(R.id.startListen);
		btnStopRecord = (Button) findViewById(R.id.stopRecord);
		btnStopListen = (Button) findViewById(R.id.stopListen);
		ipEditText = (EditText) findViewById(R.id.edittext_ip);

		//
		btnStopRecord.setEnabled(false);
		btnStopListen.setEnabled(false);

		btnExit = (Button) findViewById(R.id.btnExit);
		btnStartRecord.setOnClickListener(new View.OnClickListener() {
			public void onClick(View arg0) {

				String ipString = ipEditText.getText().toString().trim();
				NetConfig.setServerHost(ipString);
				btnStartRecord.setEnabled(false);
				btnStopRecord.setEnabled(true);
				//audioWrapper.startRecord();
				audioManager.startRecord();
			}
		});

		btnStopRecord.setOnClickListener(new View.OnClickListener() {
			public void onClick(View arg0) {
				btnStartRecord.setEnabled(true);
				btnStopRecord.setEnabled(false);
				audioManager.stopRecord();
				//audioWrapper.stopRecord();
			}
		});
		btnStartListen.setOnClickListener(new View.OnClickListener() {

			public void onClick(View arg0) {
				Log.d("btn", "onClick: startlisten");
				btnStartListen.setEnabled(false);
				btnStopListen.setEnabled(true);
				//audioWrapper.startListen();
				audioManager.startListen();
			}
		});
		btnStopListen.setOnClickListener(new View.OnClickListener() {
			public void onClick(View arg0) {
				Log.d("btn", "onClick: stoplisten");
				btnStartListen.setEnabled(true);
				btnStopListen.setEnabled(false);
				//audioWrapper.stopListen();
				audioManager.stopListen();
			}
		});
		btnExit.setOnClickListener(new View.OnClickListener() {
			public void onClick(View arg0) {
				//audioWrapper.stopListen();
				//audioWrapper.stopRecord();
				audioManager.stopListen();
				audioManager.stopRecord();
				System.exit(0);
			}
		});
	}
}
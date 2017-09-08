package com.horse.core;

import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.RatingBar;
import android.widget.TextView;

import com.horse.R;
import com.horse.screens.LobbyScreenActivity;
import com.horse.utils.HorseCache;

import java.util.ArrayList;

/**
 * Created by trhous on 8/30/2017.
 * @author TreJon House
 * @version 1.0
 * @see android.widget.ArrayAdapter
 * A custom array adapter for player
 */

public class PlayerAdapter extends ArrayAdapter<Player> {
    public PlayerAdapter(Context context, ArrayList<Player> players){
        super(context,0,players);
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent){
        // Get the data item for this position
        Player player = getItem(position);
        // Check if an existing view is being reused, otherwise inflate the view
        if (convertView == null) {
            convertView = LayoutInflater.from(getContext()).inflate(R.layout.player_list, parent, false);
        }
        // Lookup view for data population
        TextView playerName = (TextView) convertView.findViewById(R.id.playerName);
        RatingBar vip = (RatingBar) convertView.findViewById(R.id.vip_star);
        // Populate the data into the template view using the data object
        playerName.setText(player.getName());
        if(player.isVip())
            vip.setBackgroundColor(Color.parseColor("#ffe33d"));
        if(player.isNext())
            vip.setBackgroundColor(Color.parseColor("#C0F252"));
        if(player.getId().equals(HorseCache.getItem("MyDeviceId")))
            vip.setBackgroundColor(Color.parseColor("#ef0041"));
        // Return the completed view to render on screen
        return convertView;
    }
}

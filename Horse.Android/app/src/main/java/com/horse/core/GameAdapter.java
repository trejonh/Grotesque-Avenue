package com.horse.core;

import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.horse.R;
import com.horse.utils.HorseCache;

import java.util.ArrayList;

/**
 * Created by trhous on 9/8/2017.
 */

public class GameAdapter extends ArrayAdapter<Game> {

    public GameAdapter(Context context, ArrayList<Game> games){
        super(context,0,games);
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent){
        // Get the data item for this position
        Game game = getItem(position);
        if(game == null)
            return null;
        // Check if an existing view is being reused, otherwise inflate the view
        if (convertView == null) {
            convertView = LayoutInflater.from(getContext()).inflate(R.layout.game_list, parent, false);
        }
        // Lookup view for data population
        TextView gameTitle = (TextView) convertView.findViewById(R.id.gameSelectionText);
        ImageView gameImage = (ImageView) convertView.findViewById(R.id.gameSelectionImage);
        gameImage.setImageResource(game.ImageSrc);
        // Populate the data into the template view using the data object
        gameTitle.setText(game.Title);
        // Return the completed view to render on screen
        return convertView;
    }
}

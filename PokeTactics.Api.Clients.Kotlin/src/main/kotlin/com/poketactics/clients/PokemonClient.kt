package com.poketactics.clients

import com.poketactics.clients.api.PokeTacticsApiVersion1000CultureneutralPublicKeyTokennullApi as PokeTacticsClient
import com.poketactics.clients.models.KeysetPaginationResponse
import javax.inject.Inject

class PokemonClient @Inject constructor(
    private val client: PokeTacticsClient
) : IPokemonClient {

    override suspend fun getPokemon(pageSize: Int, lastPokedexOrder: Int?, lastId: Int?): KeysetPaginationResponse {
        return client.getPokemon(pageSize, lastPokedexOrder, lastId).body();
    }

    override suspend fun getPokemonSimple(pageSize: Int, lastPokedexOrder: Int?, lastId: Int?): KeysetPaginationResponse { 
        return client.getPokemonSimple(pageSize, lastPokedexOrder, lastId).body();
    }
}
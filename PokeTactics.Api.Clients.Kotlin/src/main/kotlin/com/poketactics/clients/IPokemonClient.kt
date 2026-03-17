package com.poketactics.clients

import com.poketactics.clients.models.KeysetPaginationResponse

interface IPokemonClient {
    suspend fun getPokemon(pageSize: Int, lastPokedexOrder: Int?, lastId: Int?): KeysetPaginationResponse
    suspend fun getPokemonSimple(pageSize: Int, lastPokedexOrder: Int?, lastId: Int?): KeysetPaginationResponse
}

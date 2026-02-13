<template>
  <v-app>
    <v-main>
      <v-container fluid fill-height>
        <v-row align="center" justify="center">
          <v-col cols="12" sm="8" md="4">
            <v-card class="elevation-12">
              <v-card-title class="text-h5 text-center pa-6">
                <slot name="title"></slot>
              </v-card-title>
              <v-card-text>
                <slot name="content"></slot>
              </v-card-text>
              <v-card-actions class="justify-center pa-4">
                <slot name="actions"></slot>
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
export default {
  name: 'AuthLayout'
}
</script>

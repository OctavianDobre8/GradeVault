import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ClassesRoutingModule } from './classes-routing.module';
import { ClassViewComponent } from './class-view/class-view.component';

@NgModule({
  declarations: [],
  imports: [CommonModule, ClassesRoutingModule, ClassViewComponent],
})
export class ClassesModule {}
